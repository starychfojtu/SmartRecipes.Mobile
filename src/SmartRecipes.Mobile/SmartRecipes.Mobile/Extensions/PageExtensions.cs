using System;
using System.Threading.Tasks;
using FuncSharp;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Extensions
{
    public static class PageExtensions
    {
        public static Task LoaderAction(ActivityIndicator indicator, Func<Unit, Task<Unit>> a)
        {
            indicator.IsRunning = true;
            return a(Unit.Value).Map(_ => indicator.IsRunning = false);
        }
    }
}
