using System;
using System.Collections.Generic;
using System.Linq;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.WriteModels;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using static SmartRecipes.Mobile.WriteModels.UserHandler;

namespace SmartRecipes.Mobile.ViewModels
{
    public sealed class SignInViewModel : ViewModel
    {
        private readonly Enviroment enviroment;

        public SignInViewModel(Enviroment enviroment)
        {
            this.enviroment = enviroment;
            Email = ValidatableObject.Create<string>(
                s => Mail.Create(s).IsSuccess,
                _ => RaisePropertyChanged(nameof(Email))
            );
            Password = ValidatableObject.Create<string>(
                s => Models.Password.Create(s).IsSuccess,
                _ => RaisePropertyChanged(nameof(Password))
            );
        }

        public ValidatableObject<string> Email { get; set; }

        public ValidatableObject<string> Password { get; set; }
       
        public Task<UserActionResult> SignIn()
        {
            return GetCredentials()
                .MapError(e => new SignInErrorResult(e))
                .ToCompletedTask()
                .BindTry(credentials => SignIn(credentials))
                .BindTry(a => Navigation.LogIn().Map(u => Try.Success<Unit, SignInErrorResult>(u)))
                .Map(ToUserActionResult);
        }

        private Task<ITry<IAccount, SignInErrorResult>> SignIn(Credentials credentials)
        {
            var result = UserHandler.SignIn(credentials).Execute(enviroment);
            return result.Map(r => r.MapError(e => new SignInErrorResult(e)));
        }

        private UserActionResult ToUserActionResult(ITry<Unit, SignInErrorResult> t)
        {
            return t.Match(
                s => UserActionResult.Success(),
                e => e.Match(
                    signInError => signInError.Match(
                        SignInError.InvalidCredentials, _ => UserActionResult.Error(UserMessages.InvalidCredentials()),
                        SignInError.NoConnection, _ => UserActionResult.Error(UserMessages.NoConnection())
                    ),
                    exceptions => UserActionResult.Error(UserMessages.InvalidCredentials()) // TODO: implement proper handling
                )
            );
        }

        public Task<Unit> SignUp()
        {
            return Navigation.SignUp();
        }
        
        private ITry<Credentials> GetCredentials()
        {
            return Try.Aggregate(
                Mail.Create(Email.Value),
                Models.Password.Create(Password.Value),
                Credentials.Create
            );
        }
    }

    public sealed class SignInErrorResult : Coproduct2<SignInError, IEnumerable<Exception>>
    {
        public SignInErrorResult(SignInError firstValue) : base(firstValue)
        {
        }

        public SignInErrorResult(IEnumerable<Exception> secondValue) : base(secondValue)
        {
        }
    }
}
