using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static SmartRecipes.Mobile.Extensions.PageExtensions;

namespace SmartRecipes.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage(SignUpViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            
            viewModel.BindText(EmailEntry, vm => vm.Email.Value);
            viewModel.BindErrors(EmailEntry, vm => vm.Email.IsValid);
            
            viewModel.BindText(PasswordEntry, vm => vm.Password.Value);
            viewModel.BindErrors(PasswordEntry, vm => vm.Password.IsValid);
            
            SignUpButton.Clicked += async (s, e) => await this.AlertAction(Loader, _ => viewModel.SignUp());
            SignInButton.Clicked += async (s, e) => await viewModel.SignIn();
        }
    }
}