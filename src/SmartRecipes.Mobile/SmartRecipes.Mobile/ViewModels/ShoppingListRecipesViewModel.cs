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

namespace SmartRecipes.Mobile.ViewModels
{
    public class ShoppingListRecipesViewModel : ViewModel
    {
        // TODO: This next
        
        private readonly Environment environment;
        
        private ShoppingListWithItems shoppingList;

        private IImmutableDictionary<Guid, RecipeDetail> recipeDetails;

        public ShoppingListRecipesViewModel(Environment environment)
        {
            this.environment = environment;
            recipeDetails = ImmutableDictionary.Create<Guid, RecipeDetail>();
        }

        public IEnumerable<RecipeCellViewModel> Recipes => 
            shoppingList.RecipeItems.Select(i => ToViewModel(i));

        // Initialize
        
        public override Task<Unit> InitializeAsync() =>
            ShoppingListRepository.GetRecipeItemsWithDetails(CurrentAccount)
                .Map(items => UpdateRecipeItems(items))
                .Execute(environment);
        
        // Delete recipe
            
        private Task<IOption<UserMessage>> RecipeDeleteAction(IRecipe recipe, Func<Environment, ShoppingListRecipeItemWithDetail, ITry<Task<Unit>>> action) =>
            recipeItems.First(r => r.Detail.Recipe.Equals(recipe))
                .Pipe(item => action(environment, item).Map(task => task.Map(_ => item)))
                .Map(itemTask => itemTask.Map(i => UpdateRecipeItems(recipeItems.Remove(i))))
                .MapToUserMessageAsync(_ => UserMessages.Deleted().ToOption());

        // Helpers
        
        private Unit UpdateShoppingList(ShoppingListWithItems newShoppingList) =>
            (shoppingList = newShoppingList)
                .Pipe(_ => RaisePropertyChanged(nameof(Recipes)));
        
        private RecipeCellViewModel ToViewModel(IShoppingListRecipeItem item) => 
            new RecipeCellViewModel(
                recipeDetails.Get(item.RecipeId).Get(),
                item.PersonCount.ToOption(),
                new UserAction<IRecipe>(r => RecipeDeleteAction(r, (da, i) => ShoppingListHandler.Cook(da, i)), Icon.Done(), 1),
                new UserAction<IRecipe>(r => RecipeDeleteAction(r, (da, i) => ShoppingListHandler.RemoveFromShoppingList(da, i.ShoppingListRecipeItem)), Icon.CartRemove(), 2)
            );
    }
}
