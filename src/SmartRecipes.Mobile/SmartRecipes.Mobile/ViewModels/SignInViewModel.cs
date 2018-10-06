using SmartRecipes.Mobile.WriteModels;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;
using static SmartRecipes.Mobile.WriteModels.AccountHandler;

namespace SmartRecipes.Mobile.ViewModels
{
    public sealed class SignInViewModel : ViewModel
    {
        private readonly Environment environment;

        public SignInViewModel(Environment environment)
        {
            this.environment = environment;
        }

        public string Email { get; set; }

        public string Password { get; set; }
       
        // Sign in
        
        public Task<UserActionResult> SignIn() =>
            SignIn(Email, Password)
                .Map(ToUserActionResult);

        private Task<ITry<Unit, SignInError>> SignIn(string email, string password) =>
            AccountHandler
                .SignIn(email, password)
                .Bind(u => OpenApp(u).ToReader())
                .Execute(environment);
        
        private Task<ITry<Unit, SignInError>> OpenApp(Unit u) => 
            Navigation
                .OpenApp()
                .Map(_ => Try.Success<Unit, SignInError>(u));

        private UserActionResult ToUserActionResult(ITry<Unit, SignInError> t) => 
            t.Match(
                s => UserActionResult.Success(),
                e => e.Match(
                    SignInError.InvalidCredentials, _ => UserActionResult.Error(UserMessages.InvalidCredentials()),
                    SignInError.NoConnection, _ => UserActionResult.Error(UserMessages.NoConnection())
                )
            );
        
        // Sign up
        
        public Task<Unit> SignUp() => 
            Navigation.SignUp();
    }
}
