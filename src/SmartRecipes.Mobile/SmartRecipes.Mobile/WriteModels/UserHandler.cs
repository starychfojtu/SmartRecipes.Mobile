using System;
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
    public static class UserHandler
    {
        public enum SignInError
        {
            InvalidCredentials,
            NoConnection
        }
        
        public static Reader<Environment, Task<ITry<IAccount, SignInError>>> SignIn(Credentials credentials)
        {
            var request = new SignInRequest(credentials.Email.Address, credentials.Password.Value);
            return ApiClient.Post(request).Map(r => r.Match(
                response => Try.Success<IAccount, SignInError>(ToAccount(response, credentials.Email)),
                error => Try.Error<IAccount, SignInError>(SignInError.InvalidCredentials),
                noConn => Try.Error<IAccount, SignInError>(SignInError.NoConnection)
            ));
        }

        private static IAccount ToAccount(SignInResponse response, MailAddress email)
        {
            return new Account(response.AccountId, email, new AccessToken(response.Token));
        }
        
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
