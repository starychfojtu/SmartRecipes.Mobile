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
        private readonly Environment _environment;

        private IImmutableList<ShoppingListRecipeItem> recipeItems;

        public ShoppingListRecipesViewModel(Environment environment)
        {
            this._environment = environment;
            recipeItems = ImmutableList.Create<ShoppingListRecipeItem>();
        }

        public IEnumerable<RecipeCellViewModel> Recipes
        {
            get { return recipeItems.Select(i => ToViewModel(i)); }
        }

        public override async Task InitializeAsync()
        {
            UpdateRecipeItems(await ShoppingListRepository.GetRecipeItems(CurrentAccount)(_environment));
        }
        
        private Task<IOption<UserMessage>> RecipeDeleteAction(IRecipe recipe, Func<Environment, ShoppingListRecipeItem, ITry<Task<Unit>>> action)
        {
            var item = recipeItems.First(r => r.Detail.Recipe.Equals(recipe));
            return action(_environment, item).MapToUserMessageAsync(_ =>
            {
                UpdateRecipeItems(recipeItems.Remove(item));
                return UserMessages.Deleted().ToOption();
            });
        }

        private void UpdateRecipeItems(IEnumerable<ShoppingListRecipeItem> items)
        {
            recipeItems = items.ToImmutableList();
            RaisePropertyChanged(nameof(Recipes));
        }
        
        private RecipeCellViewModel ToViewModel(ShoppingListRecipeItem item)
        {
            return new RecipeCellViewModel(
                item.Detail,
                item.RecipeInShoppingList.PersonCount.ToOption(),
                new UserAction<IRecipe>(r => RecipeDeleteAction(r, (da, i) => ShoppingListHandler.Cook(da, i)), Icon.Done(), 1),
                new UserAction<IRecipe>(r => RecipeDeleteAction(r, (da, i) => ShoppingListHandler.RemoveFromShoppingList(da, i.RecipeInShoppingList)), Icon.CartRemove(), 2)
            );
        }
    }
}
