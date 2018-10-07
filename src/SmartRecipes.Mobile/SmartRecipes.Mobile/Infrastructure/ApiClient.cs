using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FuncSharp;
using Newtonsoft.Json;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Models;
using static SmartRecipes.Mobile.Infrastructure.ApiResult;
using Option = FuncSharp.Option;

namespace SmartRecipes.Mobile.Infrastructure
{   
    public static class ApiClient
    {
        public static Monad.Reader<Environment, Task<ApiResult<SignInResponse>>> Post(SignInRequest request) =>
            Post<SignInRequest, SignInResponse>(request, "/signIn", Option.Empty<AccessToken>());
        
        public static Monad.Reader<Environment, Task<ApiResult<SignUpResponse>>> Post(SignUpRequest request) =>
            Post<SignUpRequest, SignUpResponse>(request, "/signUp", Option.Empty<AccessToken>());
        
        public static Monad.Reader<Environment, Task<ApiResult<GetShoppingListResponse>>> GetShoppingList(AccessToken token) =>
            Get<GetShoppingListResponse>(new GetShoppingListRequest(), "/shoppingList", token.ToOption());
        
        public static Monad.Reader<Environment, Task<ApiResult<ChangeFoodstuffAmountResponse>>> Post(AccessToken token, ChangeFoodstuffAmountRequest request) =>
            throw new NotImplementedException();
        
        public static Monad.Reader<Environment, Task<ApiResult<SearchFoodstuffResponse>>> Get(SearchFoodstuffRequest request) =>
            throw new NotImplementedException();

        public static Monad.Reader<Environment, Task<ApiResult<MyRecipesResponse>>> GetMyRecipes()
        {
            throw new NotImplementedException();
        }
        
        private static Monad.Reader<Environment, Task<ApiResult<TResponse>>> Post<TRequest, TResponse>(TRequest request, string route, IOption<AccessToken> token)
        {
            return env =>
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                token.Match(t => content.Headers.Add("authorization", t.Value));
                
                var response = env.HttpClient.PostAsync(route, content);
                return response.Bind(r =>
                {
                    var responseContent = r.Content.ReadAsStringAsync();
                    return responseContent.Map(c => r.IsSuccessStatusCode
                        ? Success(JsonConvert.DeserializeObject<TResponse>(c))
                        : Error<TResponse>(JsonConvert.DeserializeObject<ApiError>(c))
                    );
                });
            };
        }
        
        private static Monad.Reader<Environment, Task<ApiResult<TResponse>>> Get<TResponse>(IGetRequest request, string route, IOption<AccessToken> token)
        {
            return env =>
            {
                var queryString = request.ToQueryString().ToString();
                var baseUri = env.HttpClient.BaseAddress;
                var uriBuilder = new UriBuilder(baseUri)
                {
                    Query = queryString,
                    Path = route
                };
                var endpoint = uriBuilder.Uri;
                token.Match(t => env.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(t.Value));
                
                var response = env.HttpClient.GetAsync(endpoint.PathAndQuery);
                return response.Bind(r =>
                {
                    var responseContent = r.Content.ReadAsStringAsync();
                    return responseContent.Map(c => r.IsSuccessStatusCode
                        ? Success(JsonConvert.DeserializeObject<TResponse>(c))
                        : Error<TResponse>(JsonConvert.DeserializeObject<ApiError>(c))
                    );
                });
            };
        }
    }
}
