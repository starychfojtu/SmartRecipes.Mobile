using System.Threading.Tasks;
using System;
using SmartRecipes.Mobile.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FuncSharp;
using Monad;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.ReadModels.Dto;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;
using Try = FuncSharp.Try;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class ShoppingListHandler
    {
        // Add foodstuffs

        public enum AddFoodstuffsError
        {
            FoodstuffAlreadyAdded
        }
        
        public static Reader<Environment, Task<ITry<ShoppingListWithItems, AddFoodstuffsError>>> Add(IAccount owner, IEnumerable<IFoodstuff> foodstuffs) =>
            new AddFoodstuffsToShoppingListRequest(owner.AccessToken, foodstuffs.Select(f => f.Id))
                .Pipe(r => ApiClient.Post(r))
                .Bind(response => Process(response).Async());
        
        private static ITry<ShoppingListWithItems, AddFoodstuffsError> Process(ApiResult<AddFoodstuffsToShoppingListResponse> response) =>
            response.Match(
                s => Try.Success<ShoppingListWithItems, AddFoodstuffsError>(ToShoppingList(s)),
                e => Try.Error<ShoppingListWithItems, AddFoodstuffsError>(e.Message.Match(
                    "Item already added.", _ => AddFoodstuffsError.FoodstuffAlreadyAdded
                )),
                noConn => throw new NotImplementedException()
            );
        
        // Remove foodstuffs
        
        public enum RemoveFoodstuffsError
        {
            FoodstuffNotInShoppingList,
            FoodstuffIsRequiredInRecipe
        }
        
        public static Reader<Environment, Task<ITry<ShoppingListWithItems, RemoveFoodstuffsError>>> Remove(IAccount owner, IEnumerable<IFoodstuff> foodstuffs) =>
            new RemoveFoodstuffsFromShoppingListRequest(owner.AccessToken, foodstuffs.Select(f => f.Id))
                .Pipe(r => ApiClient.Post(r))
                .Bind(response => Process(response).Async());
        
        private static ITry<ShoppingListWithItems, RemoveFoodstuffsError> Process(ApiResult<RemoveFoodstuffsFromShoppingListResponse> response) =>
            response.Match(
                s => Try.Success<ShoppingListWithItems, RemoveFoodstuffsError>(ToShoppingList(s)),
                e => Try.Error<ShoppingListWithItems, RemoveFoodstuffsError>(e.Message.Match(
                    "Foodstuff not in list.", _ => RemoveFoodstuffsError.FoodstuffNotInShoppingList,
                    "Foodstuff is required in recipe.", _ => RemoveFoodstuffsError.FoodstuffIsRequiredInRecipe
                )),
                noConn => throw new NotImplementedException()
            );
        
        // Change amount
        
        public enum ChangeAmountError
        {
            FoodstuffNotInShoppingList,
            AmountMustBePositive
        }
        
        public static Reader<Environment, Task<ITry<ShoppingListWithItems, ChangeAmountError>>> ChangeAmount(IAccount owner, IFoodstuff foodstuff, float amount) =>
            new ChangeFoodstuffAmountRequest(owner.AccessToken, foodstuff.Id, amount)
                .Pipe(r => ApiClient.Post(r))
                .Bind(response => Process(response).Async());
        
        private static ITry<ShoppingListWithItems, ChangeAmountError> Process(ApiResult<ChangeFoodstuffAmountResponse> response) =>
            response.Match(
                s => Try.Success<ShoppingListWithItems, ChangeAmountError>(ToShoppingList(s)),
                e => Try.Error<ShoppingListWithItems, ChangeAmountError>(e.Message.Match(
                    "Foodstuff not in list.", _ => ChangeAmountError.FoodstuffNotInShoppingList,
                    "Amount must be positive.", _ => ChangeAmountError.AmountMustBePositive
                )),
                noConn => throw new NotImplementedException()
            );
            
        // Shared
        
        private static ShoppingListWithItems ToShoppingList(ShoppingListResponse response) =>
            new ShoppingListWithItems(
                ShoppingList.Create(response.Id, response.OwnerId),
                response.Items.Select(i => ShoppingListItem.Create(response.Id, i.FoodstuffId, i.Amount)),
                response.Recipes.Select(i => ShoppingListRecipeItem.Create(response.Id, i.RecipeId, i.PersonCount)
            ));
        
        // --------
        // Deprecated methods (not refactored)
        // --------
        
        public static Reader<Environment, Task<Unit>> AddToShoppingList(IRecipe recipe, IAccount owner, int personCount)
        {
            return ShoppingListRepository
                .GetRecipeItemsWithDetails(owner)
                .Select(items => items.FirstOption(i => i.Detail.Recipe.Equals(recipe)))
                .Bind(item => item.Match(
                    i => AddPersonCount(i.ShoppingListRecipeItem, personCount),
                    _ => CreateRecipeInShoppingList(recipe, owner, personCount)
                ));
        }

        public static ITry<Task<Unit>> Cook(Environment environment, ReadModels.Dto.ShoppingListRecipeItemWithDetail recipeItemWithDetail)
        {
            return null;
        }

        public static ITry<Task<Unit>> RemoveFromShoppingList(Environment environment, IShoppingListRecipeItem shoppingListRecipe)
        {
            return Try.Create(_ => environment.Db.Delete(shoppingListRecipe));
        }
        
        private static Monad.Reader<Environment, Task<Unit>> CreateRecipeInShoppingList(IRecipe recipe, IAccount owner, int personCount)
        {
            return null;
        }

        private static Monad.Reader<Environment, Task<Unit>> AddPersonCount(IShoppingListRecipeItem shoppingListRecipe, int personCount)
        {
            return env => env.Db.UpdateAsync(shoppingListRecipe.AddPersons(personCount));
        }
    }
}
