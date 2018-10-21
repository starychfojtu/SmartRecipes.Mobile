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
        
        public static Reader<Environment, Task<ShoppingListWithItems>> GetWithItems(IAccount owner) =>
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
        
        // Get required amounts 
        
        public static Reader<Environment, Task<RequiredAmounts>> GetRequiredAmounts(IAccount owner) =>
            null;

        // Deprecated methods
        
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
