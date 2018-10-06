using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static SmartRecipes.Mobile.Extensions.PageExtensions;

namespace SmartRecipes.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignInPage : ContentPage
    {
        private const double BackgroundHeight = 2057d;
        private const double BackgroundWidth = 1360d;

        public SignInPage(SignInViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            viewModel.BindText(EmailEntry, vm => vm.Email);
            viewModel.BindText(PasswordEntry, vm => vm.Password);

            SignInButton.Clicked += async (s, e) => await this.AlertAction(Loader, _ => viewModel.SignIn());
            SignUpButton.Clicked += async (s, e) => await viewModel.SignUp();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            ScaleBackground(width, height);
        }

        // TODO: move to generic utils
        private void ScaleBackground(double width, double height)
        {
            var backgroundAspectRatio = BackgroundWidth / BackgroundHeight;
            var screenAspectRation = width / height;
            var errorDeviation = 0.05;

            if (screenAspectRation > backgroundAspectRatio)
            {
                var aspectedHeight = (BackgroundHeight / BackgroundWidth) * width;
                BackgroundImage.Scale = errorDeviation + aspectedHeight / height;
            }
            else
            {
                var aspectedWidth = backgroundAspectRatio * height;
                BackgroundImage.Scale = errorDeviation + aspectedWidth / width;
            }
        }
    }
}