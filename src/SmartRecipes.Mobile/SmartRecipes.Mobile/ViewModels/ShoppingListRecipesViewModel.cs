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
        private readonly Environment environment;

        private IImmutableList<ShoppingListRecipeItemWithDetail> recipeItems;

        public ShoppingListRecipesViewModel(Environment environment)
        {
            this.environment = environment;
            recipeItems = ImmutableList.Create<ShoppingListRecipeItemWithDetail>();
        }

        public IEnumerable<RecipeCellViewModel> Recipes => 
            recipeItems.Select(i => ToViewModel(i));

        // Initialize
        
        public override Task<Unit> InitializeAsync() =>
            ShoppingListRepository.GetRecipeItemsWithDetails(CurrentAccount)
                .Map(items => UpdateRecipeItems(items))
                .Execute(environment);
            
        private Task<IOption<UserMessage>> RecipeDeleteAction(IRecipe recipe, Func<Environment, ShoppingListRecipeItemWithDetail, ITry<Task<Unit>>> action) =>
            recipeItems.First(r => r.Detail.Recipe.Equals(recipe))
                .Pipe(item => action(environment, item).Map(task => task.Map(_ => item)))
                .Map(itemTask => itemTask.Map(i => UpdateRecipeItems(recipeItems.Remove(i))))
                .MapToUserMessageAsync(_ => UserMessages.Deleted().ToOption());

        private Unit UpdateRecipeItems(IEnumerable<ShoppingListRecipeItemWithDetail> items) =>
            (recipeItems = items.ToImmutableList())
                .Pipe(_ => RaisePropertyChanged(nameof(Recipes)));
        
        private RecipeCellViewModel ToViewModel(ShoppingListRecipeItemWithDetail itemWithDetail) => 
            new RecipeCellViewModel(
                itemWithDetail.Detail,
                itemWithDetail.ShoppingListRecipeItem.PersonCount.ToOption(),
                new UserAction<IRecipe>(r => RecipeDeleteAction(r, (da, i) => ShoppingListHandler.Cook(da, i)), Icon.Done(), 1),
                new UserAction<IRecipe>(r => RecipeDeleteAction(r, (da, i) => ShoppingListHandler.RemoveFromShoppingList(da, i.ShoppingListRecipeItem)), Icon.CartRemove(), 2)
            );
    }
}
