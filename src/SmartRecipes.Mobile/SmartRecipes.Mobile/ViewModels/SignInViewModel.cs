using System;
using System.Collections.Generic;
using System.Linq;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.WriteModels;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.ReadModels;
using static SmartRecipes.Mobile.WriteModels.AccountHandler;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;

namespace SmartRecipes.Mobile.ViewModels
{
    public sealed class SignInViewModel : ViewModel
    {
        private readonly Environment environment;

        public SignInViewModel(Environment environment)
        {
            this.environment = environment;
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
       
        public Task<UserActionResult> SignIn() =>
            GetCredentials()
                .BindTry(SignIn)
                .BindTry(OpenApp)
                .Map(ToUserActionResult);

        public Task<Unit> SignUp() => 
            Navigation.SignUp();
        
        private Task<ITry<IAccount, SignInErrorResult>> SignIn(Credentials credentials) =>
            AccountHandler
                .SignIn(credentials)
                .Execute(environment)
                .Map(r => r.MapError(e => new SignInErrorResult(e)));
        
        private Task<ITry<Unit, SignInErrorResult>> OpenApp(IAccount a) => 
            Navigation
                .LogIn()
                .Map(u => Try.Success<Unit, SignInErrorResult>(u));

        private UserActionResult ToUserActionResult(ITry<Unit, SignInErrorResult> t) => 
            t.Match(
                s => UserActionResult.Success(),
                e => e.Match(
                    signInError => signInError.Match(
                        SignInError.InvalidCredentials, _ => UserActionResult.Error(UserMessages.InvalidCredentials()),
                        SignInError.NoConnection, _ => UserActionResult.Error(UserMessages.NoConnection())
                    ),
                    exceptions => exceptions.ToUserActionResult()
                )
            );
        
        private Task<ITry<Credentials, SignInErrorResult>> GetCredentials() => 
            ParseCredentials()
                .MapError(e => new SignInErrorResult(e))
                .Async();

        private ITry<Credentials> ParseCredentials() =>
            Try.Aggregate(
                Mail.Create(Email.Value),
                Models.Password.Create(Password.Value),
                Credentials.Create
            );
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
