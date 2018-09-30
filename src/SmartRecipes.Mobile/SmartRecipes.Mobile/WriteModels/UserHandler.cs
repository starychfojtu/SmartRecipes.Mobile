using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Models;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class UserHandler
    {
        public enum AuthenticationError
        {
            InvalidCredentials,
            NoConnection
        }
        
        public static Monad.Reader<Enviroment, Task<Either<IAccount, AuthenticationError>>> SignIn(SignInCredentials credentials)
        {
            return env =>
            {
                var request = new SignInRequest(credentials.Email, credentials.Password);
                var response = ApiClient.Post(request)(env.HttpClient);
                return response.Map(r => r.Match(
                    e => Right<IAccount, AuthenticationError>(AuthenticationError.InvalidCredentials),
                    a => Left<IAccount, AuthenticationError>(new Account(a.AccountId, credentials.Email, new AccessToken(a.Token)))
                ));
            };
        }
    }
}
