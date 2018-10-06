using System.Linq;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.WriteModels;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;
using static SmartRecipes.Mobile.WriteModels.AccountHandler;

namespace SmartRecipes.Mobile.ViewModels
{
    public class SignUpViewModel : ViewModel
    {
        private readonly Environment environment;

        public SignUpViewModel(Environment environment)
        {
            this.environment = environment;
            Email = new ValidatableObject<string>("", _ => true, _ => RaisePropertyChanged(nameof(Email)));
            Password = new ValidatableObject<string>("", _ => true, _ => RaisePropertyChanged(nameof(Password)));
        }

        public ValidatableObject<string> Email { get; set; }

        public ValidatableObject<string> Password { get; set; }
        
        // Sign up
        
        public Task<UserActionResult> SignUp() =>
            AccountHandler
                .SignUp(Email.Value, Password.Value)
                .Bind(u => NavigateToSignIn(u).ToReader())
                .Execute(environment)
                .Map(ToUserActionResult);
        
        private Task<ITry<Unit, SignUpError[]>> NavigateToSignIn(Unit u) => 
            Navigation
                .OpenApp()
                .Map(_ => Try.Success<Unit, SignUpError[]>(u));

        private UserActionResult ToUserActionResult(ITry<Unit, SignUpError[]> t) => 
            t.Match(
                s => UserActionResult.Success(),
                e => e.First().Match(
                    SignUpError.NoConnection, _ => UserActionResult.Error(UserMessages.NoConnection()),
                    SignUpError.InvalidEmail, _ => Email.Invalidate().Pipe(u => UserActionResult.Error()),
                    SignUpError.PasswordTooShort, _ => Password.Invalidate().Pipe(u => UserActionResult.Error()),
                    SignUpError.AccountAlreadyExists, _ => UserActionResult.Error(UserMessages.AccountAlreadyExists())
                )
            );
        
        // Sign in

        public Task<Unit> SignIn() =>
            Navigation.SignIn();
    }
}
