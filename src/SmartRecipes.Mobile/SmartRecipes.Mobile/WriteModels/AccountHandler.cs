using System.Net.Mail;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Models;
using Monad;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.WriteModels.Dto;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;
using Try = FuncSharp.Try;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class AccountHandler
    {
        // Sign in
        
        public enum SignInError
        {
            InvalidCredentials,
            NoConnection
        }
        
        public static Reader<Environment, Task<ITry<Unit, SignInError>>> SignIn(string email, string password) =>
            new SignInRequest(email, password)
                .Pipe(ApiClient.Post)
                .Bind(r => ProcessApiResult(r).Async())
                .Bind(SaveAccount);
        
        private static ITry<IAccount, SignInError> ProcessApiResult(ApiResult<SignInResponse> result) =>
            result.Match(
                r => Success(ToAccount(r)),
                error => Error(SignInError.InvalidCredentials),
                noConn => Error(SignInError.NoConnection)
            );

        private static IAccount ToAccount(SignInResponse response) =>
            Account.Create(response.AccountId, new MailAddress(response.Email), new AccessToken(response.Token));

        private static Reader<Environment, Task<ITry<Unit, SignInError>>> SaveAccount(IAccount account) =>
            env => env.Db
                .AddOrReplaceAsync(account)
                .Map(Try.Success<Unit, SignInError>);

        private static ITry<IAccount, SignInError> Error(SignInError e) =>
            Try.Error<IAccount, SignInError>(e);
        
        private static ITry<IAccount, SignInError> Success(IAccount a) =>
            Try.Success<IAccount, SignInError>(a);
        
        // Sign up
        
        public enum SignUpError
        {
            AccountAlreadyExists,
            NoConnection
        }
        
        public static Reader<Environment, Task<ITry<Unit, SignUpError>>> SignUp(SignUpParameters parameters)
        {
            var credentials = parameters.Credentials;
            var request = new SignUpRequest(credentials.Email.Address, credentials.Password.Value);
            return ApiClient.Post(request).Map(r => r.Match(
                response => Try.Success<Unit, SignUpError>(Unit.Value),
                error => Try.Error<Unit, SignUpError>(SignUpError.AccountAlreadyExists),
                noConn => Try.Error<Unit, SignUpError>(SignUpError.NoConnection)
            ));
        }
    }
}
