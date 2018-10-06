using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Models;
using Monad;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Extensions;
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
            InvalidEmail,
            PasswordTooShort,
            NoConnection
        }

        public static Reader<Environment, Task<ITry<Unit, SignUpError[]>>> SignUp(string email, string password) =>
            new SignUpRequest(email, password)
                .Pipe(ApiClient.Post)
                .Bind(r => ProcessApiResult(r).Async());

        private static ITry<Unit, SignUpError[]> ProcessApiResult(ApiResult<SignUpResponse> response) =>
            response.Match(
                r => Success(),
                error => Error(ParseErrors(error)),
                noConn => Error(new [] { SignUpError.NoConnection })
            );

        private static IEnumerable<SignUpError> ParseErrors(ApiError signUpError) =>
            signUpError.ParameterErrors.ToNonEmptyOption().Match(
                es => es.Select(e => e.Message.Match(
                    SignUpResponse.InvalidEmail, _ => SignUpError.InvalidEmail,
                    SignUpResponse.PasswordTooShort, _ => SignUpError.PasswordTooShort
                )),
                _ => signUpError.Message.Match(
                    SignUpResponse.AccountAlreadyExists, u => SignUpError.AccountAlreadyExists
                ).ToEnumerable()
            );
        
        private static ITry<Unit, SignUpError[]> Error(IEnumerable<SignUpError> errors) =>
            Try.Error<Unit, SignUpError[]>(errors.ToArray());
        
        private static ITry<Unit, SignUpError[]> Success() =>
            Try.Success<Unit, SignUpError[]>(Unit.Value);
    }
}
