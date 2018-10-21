using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SmartRecipes.Mobile.ReadModels.Dto;
using System.Linq;
using SmartRecipes.Mobile.Models;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.ReadModels;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;
using static SmartRecipes.Mobile.WriteModels.ShoppingListHandler;

namespace SmartRecipes.Mobile.ViewModels
{
    public class ShoppingListRecipesViewModel : ViewModel
    {
        private readonly Environment environment;
        
        private ShoppingListWithItems shoppingList;

        private IImmutableDictionary<Guid, RecipeDetail> recipeDetails;

        public ShoppingListRecipesViewModel(Environment environment)
        {
            this.environment = environment;
        }

        public IEnumerable<RecipeCellViewModel> Recipes => 
            shoppingList.RecipeItems.Select(i => ToViewModel(i));

        // Initialize
        
        public override Task<Unit> InitializeAsync() =>
            ShoppingListRepository.GetWithItems(CurrentAccount)
                .Map(newShoppingList => UpdateShoppingList(newShoppingList))
                .Bind(newShoppingList => RecipeRepository.GetDetails(newShoppingList.RecipeItems.Select(i => i.RecipeId)))
                .Map(details => recipeDetails = details.ToImmutableDictionary(d => d.Recipe.Id))
                .Map(_ => Unit.Value)
                .Execute(environment);
        
        // Cook recipe

        private Task<IOption<UserMessage>> Cook(IShoppingListRecipeItem item) =>
            ShoppingListHandler.Cook(CurrentAccount, item)
                .Map(r => ProcessResult(r))
                .Execute(environment);

        private IOption<UserMessage> ProcessResult(ITry<ShoppingListWithItems, CookRecipeError> result) =>
            result.Match(
                s => UpdateShoppingList(s).Pipe(_ => Option.Empty<UserMessage>()),
                e => Option.Create(e.Match(
                    CookRecipeError.NotEnoughFoodstuffsInShoppingList,
                    _ => UserMessages.NotEnoughFoodstuffsInShoppingList()
                ))
            );

        // Helpers

        private ShoppingListWithItems UpdateShoppingList(ShoppingListWithItems newShoppingList) =>
            RaisePropertyChanged(nameof(Recipes))
                .Pipe(_ => shoppingList = newShoppingList);
        
        private RecipeCellViewModel ToViewModel(IShoppingListRecipeItem item) => 
            new RecipeCellViewModel(
                recipeDetails.Get(item.RecipeId).Get(),
                item.PersonCount.ToOption(),
                new UserAction<IRecipe>(_ => Cook(item), Icon.Done(), 1),
                new UserAction<IRecipe>(_ => Cook(item), Icon.CartRemove(), 2) // TODO: Implement remove action
            );
    }
}
