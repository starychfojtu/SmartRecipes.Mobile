using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.WriteModels;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;

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
            var errorResult = Task.FromResult(UserActionResult.Error(UserMessages.InvalidCredentials()));
            if (FormIsValid)
            {
                var credentials = new SignInCredentials(Email.Data, Password.Data);
                var authResult = UserHandler.SignIn(credentials).Execute(enviroment);
                return authResult.Bind(r => r.Match(
                    a =>
                    {
                        return enviroment.Db.Seed()
                            .Bind(_ => Navigation.LogIn())
                            .Map(_ => UserActionResult.Success());
                    },
                    e => errorResult
                ));
            }
            
            return errorResult;
        }

        public Task<Unit> SignUp()
        {
            return Navigation.SignUp();
        }
        
        private bool FormIsValid
        {
            get { return Email.IsValid && Password.IsValid; }
        }
    }
}
