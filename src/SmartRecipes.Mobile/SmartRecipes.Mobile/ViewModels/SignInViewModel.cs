using System.Net.Security;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.WriteModels;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using static SmartRecipes.Mobile.WriteModels.UserHandler;

namespace SmartRecipes.Mobile.ViewModels
{
    public class SignInViewModel : ViewModel
    {
        private readonly Enviroment enviroment;

        public SignInViewModel(Enviroment enviroment)
        {
            this.enviroment = enviroment;
            Email = ValidatableObject.Create<string>(
                s => Validation.NotEmpty(s) && Validation.IsEmail(s),
                _ => RaisePropertyChanged(nameof(Email))
            );
            Password = ValidatableObject.Create<string>(
                s => Validation.NotEmpty(s),
                _ => RaisePropertyChanged(nameof(Password))
            );
        }

        public ValidatableObject<string> Email { get; set; }

        public ValidatableObject<string> Password { get; set; }
       
        public Task<UserActionResult> SignIn()
        {
            var invalidCredentials = Task.FromResult(UserActionResult.Error(UserMessages.InvalidCredentials()));
            var noConnection = Task.FromResult(UserActionResult.Error(UserMessages.NoConnection()));
            
            if (FormIsValid)
            {
                var credentials = new SignInCredentials(Email.Data, Password.Data);
                var authResult = UserHandler.SignIn(credentials).Execute(enviroment);
                return authResult.Bind(r => r.Match(
                    a => OnSignedIn(),
                    e => e.Match(
                        AuthenticationError.InvalidCredentials, _ => invalidCredentials,
                        AuthenticationError.NoConnection, _ => noConnection
                    )
                ));
            }
            
            return invalidCredentials;
        }

        public Task<Unit> SignUp()
        {
            return Navigation.SignUp();
        }

        private Task<UserActionResult> OnSignedIn()
        {
            return enviroment.Db.Seed()
                .Bind(_ => Navigation.LogIn())
                .Map(_ => UserActionResult.Success());
        }
        
        private bool FormIsValid
        {
            get { return Email.IsValid && Password.IsValid; }
        }
    }
}
