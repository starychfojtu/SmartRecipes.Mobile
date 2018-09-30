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
        public static Monad.Reader<Enviroment, Task<AuthenticationResult>> SignIn(SignInCredentials credentials)
        {
            return env =>
            {
                var request = new SignInRequest(credentials.Email, credentials.Password);
                var response = ApiClient.Post(request)(env.HttpClient);
                return response.Map(r => r.Match(
                    e => new AuthenticationResult(success: false, token: None),
                    s => new AuthenticationResult(s.IsAuthorized, s.Token)
                ));
            };
        }
    }

    public class AuthenticationResult
    {
        public AuthenticationResult(bool success, Option<string> token)
        {
            Success = success;
            Token = token;
        }

        public bool Success { get; }

        public Option<string> Token { get; }
    }
}
