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
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;
using ShoppingListItem = SmartRecipes.Mobile.Models.ShoppingListItem;
using ShoppingListRecipeItem = SmartRecipes.Mobile.Models.ShoppingListRecipeItem;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class ShoppingListHandler
    {
        public static Monad.Reader<Environment, Task<Unit>> AddToShoppingList(IRecipe recipe, IAccount owner, int personCount)
        {
            return  ShoppingListRepository
                .GetRecipeItems(owner)
                .Select(items => items.FirstOption(i => i.Detail.Recipe.Equals(recipe)))
                .Bind(item => item.Match(
                    i => AddPersonCount(i.ShoppingListRecipeItem, personCount),
                    _ => CreateRecipeInShoppingList(recipe, owner, personCount)
                ));
        }
        
        public static IShoppingListItem Increase(ShoppingListItemWithFoodstuff itemWithFoodstuff)
        {
            return ChangeAmount((a1, a2) => Amount.Add(a1, a2), itemWithFoodstuff);
        }

        public static IShoppingListItem Decrease(ShoppingListItemWithFoodstuff itemWithFoodstuff)
        {
            return ChangeAmount((a1, a2) => Amount.Substract(a1, a2), itemWithFoodstuff);
        }

        public static ITry<Task<Unit>> Cook(Environment environment, ReadModels.Dto.ShoppingListRecipeItemWithDetail recipeItemWithDetail)
        {
            return Try.Create(_ =>
            {
                var ownerId = recipeItemWithDetail.ShoppingListRecipeItem.ShoppingListId;
                var requiredAmounts = ShoppingListRepository.GetRequiredAmounts(recipeItemWithDetail);
                var shoppingListItemsTask = ShoppingListRepository.GetItems(ownerId)(environment);
                var itemDictionaryTask = shoppingListItemsTask.Map(
                    items => items.ToImmutableDictionary(i => i.Foodstuff, i => i.Item)
                );

                var substractedItemsTask = itemDictionaryTask.Map(dict =>
                {
                    return requiredAmounts.Aggregate(dict.ToOption(), (d, kvp) => d.FlatMap(items =>
                    {
                        var (foodstuff, requiredAmount) = kvp;
                        var item = items[foodstuff];

                        if (Amount.IsLessThan(item.Amount, requiredAmount))
                        {
                            return Option.Empty<ImmutableDictionary<IFoodstuff, IShoppingListItem>>();
                        }

                        var newAmount = Amount.Substract(item.Amount, requiredAmount).GetOrElse(item.Amount);
                        return Option.Create(items.SetItem(foodstuff, item.WithAmount(newAmount)));
                    }));
                });
                 
                return substractedItemsTask.Bind(items =>
                {
                    var itemsToUpdate = items.Get(u => new InvalidOperationException("Not enought ingredients in shopping list."));
                    return environment.Db.UpdateAsync(itemsToUpdate.Values);
                });
            }).FlatMap(_ => RemoveFromShoppingList(environment, recipeItemWithDetail.ShoppingListRecipeItem));
        }

        public static ITry<Task<Unit>> RemoveFromShoppingList(Environment environment, IShoppingListRecipeItem shoppingListRecipe)
        {
            return Try.Create(_ => environment.Db.Delete(shoppingListRecipe));
        }
        
        public static ITry<Task<Unit>> RemoveFromShoppingList(Environment environment, ReadModels.Dto.ShoppingListItemWithFoodstuff itemWithFoodstuff, IAccount owner)
        {
            return Try.Create(_ =>
            {
                var requiredAmounts = ShoppingListRepository.GetRequiredAmounts(owner)(environment);
                return requiredAmounts.Bind(amounts => amounts.ContainsKey(itemWithFoodstuff.Foodstuff)
                    ? throw new InvalidOperationException("Cannot remove ingredient of shoppingListRecipe in shopping list. Remvoe the shoppingListRecipe first.")
                    : environment.Db.Delete(itemWithFoodstuff.Item)
                );
            });
        }

        public static Task<IEnumerable<ShoppingListItemWithFoodstuff>> AddToShoppingList(Environment environment, IAccount owner, IEnumerable<IFoodstuff> foodstuffs)
        {
            throw new NotImplementedException();
//            return
//                from shoppingListItems in ShoppingListRepository.GetItems(owner.Id)(environment)
//                let alreadyAddedFoodstuffs = shoppingListItems.Select(i => i.Foodstuff)
//                let newFoodstuffs = foodstuffs.Except(alreadyAddedFoodstuffs).ToImmutableDictionary(f => f.Id, f => f)
//                let newItemAmounts = newFoodstuffs.Values.Select(f => ShoppingListItem.Create(owner, f, f.BaseAmount)).ToImmutableList()
//                from _1 in environment.Db.AddAsync(newItemAmounts)
//                from _2 in Update(environment, newItemAmounts)
//                select newItemAmounts.Select(fa => new ShoppingListItem(newFoodstuffs[fa.FoodstuffId], fa));
        }
        
        public static Task<Unit> Update(Environment environment, IShoppingListItem item)
        {
            return Update(environment, ImmutableList.Create(item));
        }

        public static async Task<Unit> Update(Environment environment, IImmutableList<IShoppingListItem> itemAmounts)
        {
            foreach (var itemAmount in itemAmounts)
            {
                var request = new ChangeFoodstuffAmountRequest(itemAmount.FoodstuffId, itemAmount.Amount);
                var response = await ApiClient.Post(request)(environment);
            }

            await environment.Db.UpdateAsync((IEnumerable<IShoppingListItem>) itemAmounts);
            return Unit.Value;
        }
        
        private static IShoppingListItem ChangeAmount(Func<IAmount, IAmount, IOption<IAmount>> action, ReadModels.Dto.ShoppingListItemWithFoodstuff itemWithFoodstuff)
        {
            var newAmount = action(itemWithFoodstuff.Item.Amount, itemWithFoodstuff.Foodstuff.AmountStep).Get();
            return itemWithFoodstuff.Item.WithAmount(newAmount);
        }
        
        private static Monad.Reader<Environment, Task<Unit>> CreateRecipeInShoppingList(IRecipe recipe, IAccount owner, int personCount)
        {
            return env =>
            {
                var newItemAmounts = GetRecipeComplementOfShoppingList(recipe, owner)(env);

                var recipeTask = env.Db.AddAsync(ShoppingListRecipeItem.Create(recipe, owner, personCount).ToEnumerable());
                var amountsTask = newItemAmounts.Map(a => env.Db.AddAsync(a));

                return recipeTask.Bind(_ => amountsTask).ToUnit();
            };
        }

        private static Monad.Reader<Environment, Task<Unit>> AddPersonCount(IShoppingListRecipeItem shoppingListRecipe, int personCount)
        {
            return env => env.Db.UpdateAsync(shoppingListRecipe.AddPersons(personCount));
        }

        private static Monad.Reader<Environment, Task<IEnumerable<IShoppingListItem>>> GetRecipeComplementOfShoppingList(IRecipe recipe, IAccount owner)
        {
            return
                from ingredients in RecipeRepository.GetIngredients(recipe)
                from items in ShoppingListRepository.GetItems(owner.Id)
                let foodstuffs = ingredients.Select(i => i.Foodstuff)
                let addedFoodstuffs = items.Select(i => i.Foodstuff)
                select foodstuffs.Except(addedFoodstuffs).Select(f => ShoppingListItem.Create(owner, f, Amount.Zero(f.BaseAmount.Unit)));
        }
    }
}
