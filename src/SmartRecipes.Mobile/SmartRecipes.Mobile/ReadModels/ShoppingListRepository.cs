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
using ShoppingListRecipeItem = SmartRecipes.Mobile.Models.ShoppingListRecipeItem;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class ShoppingListRepository
    {
        // Get shopping list
        
        public static Reader<Environment, Task<ShoppingListWithItems>> GetShoppingListWithItems(IAccount owner) =>
            Repository.RetrievalAction(
                ApiClient.GetShoppingList(owner.AccessToken),
                GetShoppingListWithItemsFromDb(owner),
                response => ToShoppingList(response),
                list => list.Items.Concat<object>(list.RecipeItems).Concat(list.ShoppingList.ToEnumerable())
            );
        
        private static Reader<Environment, Task<ShoppingListWithItems>> GetShoppingListWithItemsFromDb(IAccount owner) =>
            from shoppingList in GetShoppingList(owner.Id)
            from items in GetShoppingListItems(shoppingList.Id)
            from recipeItems in GetShoppingListRecipeItems(shoppingList.Id)
            select new ShoppingListWithItems(shoppingList, items, recipeItems);
        
        private static Reader<Environment, Task<IShoppingList>> GetShoppingList(Guid ownerId) =>
            env => env.Db.ShoppingLists
                .Where(i => i.OwnerId == ownerId)
                .FirstAsync()
                .Map(list => list as IShoppingList);
        
        private static Reader<Environment, Task<IEnumerable<IShoppingListItem>>> GetShoppingListItems(Guid shoppingListId) =>
            env => env.Db.ShoppingListItems
                .Where(i => i.ShoppingListId == shoppingListId)
                .ToEnumerableAsync<ShoppingListItem, IShoppingListItem>();
        
        private static Reader<Environment, Task<IEnumerable<IShoppingListRecipeItem>>> GetShoppingListRecipeItems(Guid shoppingListId) =>
            env => env.Db.ShoppingListRecipeItems
                .Where(i => i.ShoppingListId == shoppingListId)
                .ToEnumerableAsync<ShoppingListRecipeItem, IShoppingListRecipeItem>();

        // Get recipe items
        
        public static Reader<Environment, Task<IEnumerable<ShoppingListRecipeItemWithDetail>>> GetRecipeItemsWithDetails(IAccount owner) =>
            from recipesInList in GetRecipesInShoppingList(owner)
            from recipes in RecipeRepository.Get(recipesInList.Select(r => r.RecipeId))
            from details in RecipeRepository.GetDetails(recipes)
            select recipesInList.Join(details, r => r.RecipeId, d => d.Recipe.Id, (r, d) => new ShoppingListRecipeItemWithDetail(d, r));
        
        //

        public static Reader<Environment, Task<RequiredAmounts>> GetRequiredAmounts(IAccount owner) =>
            null;
//            GetRecipeItemsWithDetails(owner).Map(items => items.Aggregate(
//                ImmutableDictionary.Create<IFoodstuff, IAmount>(),
//                (r, item) => r.Merge(GetRequiredAmounts(item), (a1, a2) => Amount.Add(a1, a2).GetOrElse(a2))
//            ));
        
        public static ImmutableDictionary<IFoodstuff, IAmount> GetRequiredAmounts(ShoppingListRecipeItemWithDetail itemWithDetail)
        {
            return null;
//            var result = ImmutableDictionary.Create<IFoodstuff, IAmount>();
//            return itemWithDetail.Detail.Ingredients.Aggregate(result, (tempResult, i) =>
//            {
//                var personCountRatio = itemWithDetail.ShoppingListRecipeItem.PersonCount / itemWithDetail.Detail.Recipe.PersonCount;
//                var newAmount = i.float.WithCount(i.Amount.Count * personCountRatio); 
//                var totalAmount = tempResult.ContainsKey(i.Foodstuff)
//                    ? Amount.Add(tempResult[i.Foodstuff], newAmount).GetOrElse(newAmount)
//                    : newAmount;
//                return tempResult.SetItem(i.Foodstuff, totalAmount);
//            });
        }

        private static Reader<Environment, Task<IEnumerable<IFoodstuff>>> GetFoodstuffs(IEnumerable<Guid> ids) =>
            env => env.Db.Foodstuffs
                .Where(f => ids.Contains(f.Id))
                .ToEnumerableAsync<Foodstuff, IFoodstuff>();

        private static Reader<Environment, Task<IEnumerable<IShoppingListRecipeItem>>> GetRecipesInShoppingList(IAccount owner) =>
            env => env.Db.ShoppingListRecipeItems
                .Where(r => r.ShoppingListId == owner.Id)
                .ToEnumerableAsync<ShoppingListRecipeItem, IShoppingListRecipeItem>();
        
        private static ShoppingListWithItems ToShoppingList(GetShoppingListResponse response) =>
            new ShoppingListWithItems(
                ShoppingList.Create(response.Id, response.OwnerId),
                response.Items.Select(i => ToShoppingListItem(i, response.Id)),
                response.Recipes.Select(i => ToShoppingListRecipeItem(i, response.Id))
            );

        private static IShoppingListItem ToShoppingListItem(GetShoppingListResponse.Item item, Guid shoppingListId) =>
            ShoppingListItem.Create(shoppingListId, item.FoodstuffId, item.Amount);
        
        private static IShoppingListRecipeItem ToShoppingListRecipeItem(GetShoppingListResponse.RecipeItem item, Guid shoppingListId) =>
            ShoppingListRecipeItem.Create(shoppingListId, item.RecipeId, item.PersonCount);
    }
}
