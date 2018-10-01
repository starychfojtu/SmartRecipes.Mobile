using System;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Models;
using Monad;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Extensions;
using Try = FuncSharp.Try;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class UserHandler
    {
        public enum AuthenticationError
        {
            InvalidCredentials,
            NoConnection
        }
        
        public static Reader<Enviroment, Task<ITry<IAccount, AuthenticationError>>> SignIn(SignInCredentials credentials)
        {
            var request = new SignInRequest(credentials.Email, credentials.Password);
            return ApiClient.Post(request).Map(r => r.Match(
                response => Try.Success<IAccount, AuthenticationError>(ToAccount(response, credentials.Email)),
                error => Try.Error<IAccount, AuthenticationError>(AuthenticationError.InvalidCredentials),
                noConn => Try.Error<IAccount, AuthenticationError>(AuthenticationError.NoConnection)
            ));
        }

        private static IAccount ToAccount(SignInResponse response, string email)
        {
            return new Account(response.AccountId, email, new AccessToken(response.Token));
        }
    }
}
