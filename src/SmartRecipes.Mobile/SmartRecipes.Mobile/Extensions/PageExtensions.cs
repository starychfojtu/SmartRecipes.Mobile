using System;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Infrastructure;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Extensions
{
    public static class PageExtensions
    {
        public static Task<Unit> LoaderAction(ActivityIndicator indicator, Func<Unit, Task<Unit>> a)
        {
            indicator.IsRunning = true;
            return a(Unit.Value).Map(_ => indicator.IsRunning = false).ToUnit();
        }

        public static Task<Unit> AlertAction(this ContentPage page, ActivityIndicator loader, Func<Unit, Task<UserActionResult>> a) =>
            LoaderAction(loader, _ => a(Unit.Value)
                .Bind(r => r.Message
                    .Map(m => page.DisplayAlert(m.Title, m.Text, cancel: "Ok").ToUnit())
                    .GetOrElse(Unit.Value.Async())));
    }
}
