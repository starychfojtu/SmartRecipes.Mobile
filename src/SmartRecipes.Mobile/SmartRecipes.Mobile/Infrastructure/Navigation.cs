using System.Collections.Generic;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Pages;
using SmartRecipes.Mobile.ReadModels.Dto;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Infrastructure
{
    public static class Navigation
    {
        public static Task<Unit> LogIn()
        {
            return PageFactory.GetPageAsync<AppContainer>()
                .Map(c => Application.Current.MainPage = new NavigationPage(c))
                .ToUnit();
        }

        public static Task<Unit> SignUp()
        {
            return PageFactory.GetPageAsync<SignUpPage>()
                .Map(p => Application.Current.MainPage = p)
                .ToUnit();
        }

        public static async Task CreateRecipe()
        {
            var page = await PageFactory.GetPageAsync<EditRecipePage>();
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }

        public static async Task EditRecipe(RecipeDetail recipe)
        {
            var page = await PageFactory.GetEditRecipePage(recipe);
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }

        public static async Task<IEnumerable<IFoodstuff>> SelectFoodstuffDialog()
        {
            var selected = new TaskCompletionSource<IEnumerable<IFoodstuff>>();
            var page = await PageFactory.GetPageAsync<FoodstuffSearchPage>();

            page.SelectingEnded += (s, e) => selected.SetResult(e.Selected);

            await Application.Current.MainPage.Navigation.PushAsync(page);

            return await selected.Task;
        }
    }
}
