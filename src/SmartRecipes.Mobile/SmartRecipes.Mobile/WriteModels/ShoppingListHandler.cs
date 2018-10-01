using SmartRecipes.Mobile.ApiDto;
using System.Threading.Tasks;
using System;
using SmartRecipes.Mobile.Models;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.ReadModels.Dto;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class ShoppingListHandler
    {
        public static Monad.Reader<Enviroment, Task<Unit>> AddToShoppingList(IRecipe recipe, IAccount owner, int personCount)
        {
            return  ShoppingListRepository
                .GetRecipeItems(owner)
                .Select(items => items.FirstOption(i => i.Detail.Recipe.Equals(recipe)))
                .Bind(item => item.Match(
                    i => AddPersonCount(i.RecipeInShoppingList, personCount),
                    _ => CreateRecipeInShoppingList(recipe, owner, personCount)
                ));
        }
        
        public static IShoppingListItemAmount Increase(ShoppingListItem item)
        {
            return ChangeAmount((a1, a2) => Amount.Add(a1, a2), item);
        }

        public static IShoppingListItemAmount Decrease(ShoppingListItem item)
        {
            return ChangeAmount((a1, a2) => Amount.Substract(a1, a2), item);
        }

        public static ITry<Task<Unit>> Cook(Enviroment enviroment, ShoppingListRecipeItem recipeItem)
        {
            return Try.Create(_ =>
            {
                var ownerId = recipeItem.RecipeInShoppingList.ShoppingListOwnerId;
                var requiredAmounts = ShoppingListRepository.GetRequiredAmounts(recipeItem);
                var shoppingListItemsTask = ShoppingListRepository.GetItems(ownerId)(enviroment);
                var itemDictionaryTask = shoppingListItemsTask.Map(
                    items => items.ToImmutableDictionary(i => i.Foodstuff, i => i.ItemAmount)
                );

                var substractedItemsTask = itemDictionaryTask.Map(dict =>
                {
                    return requiredAmounts.Aggregate(dict.ToOption(), (d, kvp) => d.FlatMap(items =>
                    {
                        var (foodstuff, requiredAmount) = kvp;
                        var item = items[foodstuff];

                        if (Amount.IsLessThan(item.Amount, requiredAmount))
                        {
                            return Option.Empty<ImmutableDictionary<IFoodstuff, IShoppingListItemAmount>>();
                        }

                        var newAmount = Amount.Substract(item.Amount, requiredAmount).GetOrElse(item.Amount);
                        return Option.Create(items.SetItem(foodstuff, item.WithAmount(newAmount)));
                    }));
                });
                 
                return substractedItemsTask.Bind(items =>
                {
                    var itemsToUpdate = items.Get(u => new InvalidOperationException("Not enought ingredients in shopping list."));
                    return enviroment.Db.UpdateAsync(itemsToUpdate.Values);
                });
            }).FlatMap(_ => RemoveFromShoppingList(enviroment, recipeItem.RecipeInShoppingList));
        }

        public static ITry<Task<Unit>> RemoveFromShoppingList(Enviroment enviroment, IRecipeInShoppingList recipe)
        {
            return Try.Create(_ => enviroment.Db.Delete(recipe));
        }
        
        public static ITry<Task<Unit>> RemoveFromShoppingList(Enviroment enviroment, ShoppingListItem item, IAccount owner)
        {
            return Try.Create(_ =>
            {
                var requiredAmounts = ShoppingListRepository.GetRequiredAmounts(owner)(enviroment);
                return requiredAmounts.Bind(amounts => amounts.ContainsKey(item.Foodstuff)
                    ? throw new InvalidOperationException("Cannot remove ingredient of recipe in shopping list. Remvoe the recipe first.")
                    : enviroment.Db.Delete(item.ItemAmount)
                );
            });
        }

        public static Task<IEnumerable<ShoppingListItem>> AddToShoppingList(Enviroment enviroment, IAccount owner, IEnumerable<IFoodstuff> foodstuffs)
        {
            throw new NotImplementedException();
//            return
//                from shoppingListItems in ShoppingListRepository.GetItems(owner.Id)(enviroment)
//                let alreadyAddedFoodstuffs = shoppingListItems.Select(i => i.Foodstuff)
//                let newFoodstuffs = foodstuffs.Except(alreadyAddedFoodstuffs).ToImmutableDictionary(f => f.Id, f => f)
//                let newItemAmounts = newFoodstuffs.Values.Select(f => ShoppingListItemAmount.Create(owner, f, f.BaseAmount)).ToImmutableList()
//                from _1 in enviroment.Db.AddAsync(newItemAmounts)
//                from _2 in Update(enviroment, newItemAmounts)
//                select newItemAmounts.Select(fa => new ShoppingListItem(newFoodstuffs[fa.FoodstuffId], fa));
        }

        public static async Task<Unit> Update(Enviroment enviroment, IImmutableList<IShoppingListItemAmount> itemAmounts)
        {
            foreach (var itemAmount in itemAmounts)
            {
                var request = new ChangeFoodstuffAmountRequest(itemAmount.FoodstuffId, itemAmount.Amount);
                var response = await ApiClient.Post(request)(enviroment.HttpClient);
            }

            await enviroment.Db.UpdateAsync((IEnumerable<IShoppingListItemAmount>) itemAmounts);
            return Unit.Value;
        }
        
        private static IShoppingListItemAmount ChangeAmount(Func<IAmount, IAmount, IOption<IAmount>> action, ShoppingListItem item)
        {
            var newAmount = action(item.ItemAmount.Amount, item.Foodstuff.AmountStep).Get();
            return item.ItemAmount.WithAmount(newAmount);
        }
        
        private static Monad.Reader<Enviroment, Task<Unit>> CreateRecipeInShoppingList(IRecipe recipe, IAccount owner, int personCount)
        {
            return env =>
            {
                var newItemAmounts = GetRecipeComplementOfShoppingList(recipe, owner)(env);

                var recipeTask = env.Db.AddAsync(RecipeInShoppingList.Create(recipe, owner, personCount).ToEnumerable());
                var amountsTask = newItemAmounts.Map(a => env.Db.AddAsync(a));

                return recipeTask.Bind(_ => amountsTask).ToUnit();
            };
        }

        private static Monad.Reader<Enviroment, Task<Unit>> AddPersonCount(IRecipeInShoppingList recipe, int personCount)
        {
            return env => env.Db.UpdateAsync(recipe.AddPersons(personCount));
        }

        private static Monad.Reader<Enviroment, Task<IEnumerable<IShoppingListItemAmount>>> GetRecipeComplementOfShoppingList(IRecipe recipe, IAccount owner)
        {
            return
                from ingredients in RecipeRepository.GetIngredients(recipe)
                from items in ShoppingListRepository.GetItems(owner.Id)
                let foodstuffs = ingredients.Select(i => i.Foodstuff)
                let addedFoodstuffs = items.Select(i => i.Foodstuff)
                select foodstuffs.Except(addedFoodstuffs).Select(f => ShoppingListItemAmount.Create(owner, f, Amount.Zero(f.BaseAmount.Unit)));
        }
    }
}
