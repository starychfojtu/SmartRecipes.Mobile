using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Models;
using System.Collections.Immutable;
using FuncSharp;
using Monad;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;
using ShoppingListItem = SmartRecipes.Mobile.Models.ShoppingListItem;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class ShoppingListRepository
    {
        public static Reader<Environment, Task<IEnumerable<ShoppingListItemWithFoodstuff>>> GetItems(Guid ownerId) =>
            Repository.RetrievalAction(
                ApiClient.GetShoppingList(),
                GetShoppingListItems(),
                response => response.Items.Select(i => ToShoppingListItem(i, ownerId)),
                items => items.SelectMany(i => new object[] { i.Foodstuff, i.Item })
            );

        public static Reader<Environment, Task<IEnumerable<ShoppingListRecipeItem>>> GetRecipeItems(IAccount owner) =>
            from recipesInList in GetRecipesInShoppingList(owner)
            from recipes in RecipeRepository.Get(recipesInList.Select(r => r.RecipeId))
            from details in RecipeRepository.GetDetails(recipes)
            select recipesInList.Join(details, r => r.RecipeId, d => d.Recipe.Id, (r, d) => new ShoppingListRecipeItem(d, r));

        public static Reader<Environment, Task<ImmutableDictionary<IFoodstuff, IAmount>>> GetRequiredAmounts(IAccount owner) =>
            GetRecipeItems(owner).Map(items => items.Aggregate(
                ImmutableDictionary.Create<IFoodstuff, IAmount>(),
                (r, item) => r.Merge(GetRequiredAmounts(item), (a1, a2) => Amount.Add(a1, a2).GetOrElse(a2))
            ));
        
        public static ImmutableDictionary<IFoodstuff, IAmount> GetRequiredAmounts(ShoppingListRecipeItem item)
        {
            var result = ImmutableDictionary.Create<IFoodstuff, IAmount>();
            return item.Detail.Ingredients.Aggregate(result, (tempResult, i) =>
            {
                var personCountRatio = item.RecipeInShoppingList.PersonCount / item.Detail.Recipe.PersonCount;
                var newAmount = i.Amount.WithCount(i.Amount.Count * personCountRatio); 
                var totalAmount = tempResult.ContainsKey(i.Foodstuff)
                    ? Amount.Add(tempResult[i.Foodstuff], newAmount).GetOrElse(newAmount)
                    : newAmount;
                return tempResult.SetItem(i.Foodstuff, totalAmount);
            });
        }

        private static Reader<Environment, Task<IEnumerable<Dto.ShoppingListItemWithFoodstuff>>> GetShoppingListItems() =>
            from itemAmounts in GetShoppingListItemAmounts()
            from foodstuffs in GetFoodstuffs(itemAmounts.Select(i => i.FoodstuffId))
            select itemAmounts.Join(foodstuffs, i => i.FoodstuffId, f => f.Id, (i, f) => new Dto.ShoppingListItemWithFoodstuff(f, i));

        private static Reader<Environment, Task<IEnumerable<IShoppingListItem>>> GetShoppingListItemAmounts() =>
            env => env.Db.ShoppingListItems.ToEnumerableAsync<ShoppingListItem, IShoppingListItem>();

        private static Reader<Environment, Task<IEnumerable<IFoodstuff>>> GetFoodstuffs(IEnumerable<Guid> ids) =>
            env => env.Db.Foodstuffs.Where(f => ids.Contains(f.Id)).ToEnumerableAsync<Foodstuff, IFoodstuff>();

        private static Reader<Environment, Task<IEnumerable<IRecipeInShoppingList>>> GetRecipesInShoppingList(IAccount owner) =>
            env => env.Db.RecipeInShoppingLists
                .Where(r => r.ShoppingListOwnerId == owner.Id)
                .ToEnumerableAsync<RecipeInShoppingList, IRecipeInShoppingList>();
        
        private static Dto.ShoppingListItemWithFoodstuff ToShoppingListItem(ShoppingListResponse.Item i, Guid ownerId)
        {
            var foodstuff = Foodstuff.Create(
                i.FoodstuffDto.Id,
                i.FoodstuffDto.Name,
                i.FoodstuffDto.ImageUrl,
                i.FoodstuffDto.BaseAmount,
                i.FoodstuffDto.AmountStep
            );
            return new Dto.ShoppingListItemWithFoodstuff(foodstuff, ShoppingListItem.Create(i.Id, ownerId, foodstuff.Id, i.Amount));
        }
    }
}
