using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ViewModels
{
    public class MyRecipesViewModel : ViewModel
    {
        private readonly Environment environment;

        public MyRecipesViewModel(Environment environment)
        {
            this.environment = environment;
        }

        public IEnumerable<RecipeCellViewModel> Recipes { get; private set; }

        public override Task<Unit> InitializeAsync() =>
            UpdateRecipesAsync();

        public Task AddRecipe()
        {
            return Navigation.CreateRecipe();
        }

        public async Task<Unit> UpdateRecipesAsync()
        {
            var recipeDetails = await RecipeRepository.GetMyRecipeDetails()(environment);
            Recipes = recipeDetails.Select(detail => new RecipeCellViewModel(
                detail,
                Option.Empty<int>(),
                new UserAction<IRecipe>(r => AddToShoppingList(r), Icon.CartAdd(), 1),
                new UserAction<IRecipe>(r => EditRecipe(r), Icon.Edit(), 2),
                new UserAction<IRecipe>(r => Task.FromResult(DeleteRecipe(r)), Icon.Delete(), 3)
            ));
            RaisePropertyChanged(nameof(Recipes));
            return Unit.Value;
        }

        public async Task<IOption<UserMessage>> EditRecipe(IRecipe recipe)
        {
            var detail = await RecipeRepository.GetDetail(recipe)(environment);
            await Navigation.EditRecipe(detail);
            return Option.Empty<UserMessage>();
        }

        public IOption<UserMessage> DeleteRecipe(IRecipe recipe)
        {
            return MyRecipesHandler.Delete(environment, recipe)
                .FlatMap(u => Try.Create(_ => InitializeAsync().ToUnit()))
                .MapToUserMessage(_ => UserMessages.Deleted().ToOption());
        }

        private Task<IOption<UserMessage>> AddToShoppingList(IRecipe recipe)
        {
            return ShoppingListHandler
                .AddToShoppingList(recipe, CurrentAccount, recipe.PersonCount)(environment)
                .Map(_ => UserMessages.Added().ToOption());
        }
    }
}
