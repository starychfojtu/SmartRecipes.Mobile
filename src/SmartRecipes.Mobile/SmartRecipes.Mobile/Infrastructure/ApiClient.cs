using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Extensions;
using static SmartRecipes.Mobile.Infrastructure.ApiResult;

namespace SmartRecipes.Mobile.Infrastructure
{   
    public static class ApiClient
    {
        public static Monad.Reader<HttpClient, Task<ApiResult<SignInResponse>>> Post(SignInRequest request)
        {
            return Post<SignInRequest, SignInResponse>(request, "/signIn");
        }
        
        public static Monad.Reader<HttpClient, Task<ApiResult<SignUpResponse>>> Post(SignUpRequest request)
        {
            return Post<SignUpRequest, SignUpResponse>(request, "/signUp");
        }
        
        public static Monad.Reader<HttpClient, Task<ApiResult<ChangeFoodstuffAmountResponse>>> Post(ChangeFoodstuffAmountRequest request)
        {
            return Post<ChangeFoodstuffAmountRequest, ChangeFoodstuffAmountResponse>(request, "/signIn");
        }
        
        private static Monad.Reader<HttpClient, Task<ApiResult<TResponse>>> Post<TRequest, TResponse>(TRequest request, string route)
        {
            return client =>
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = client.PostAsync(route, content);
                return response.Bind(r =>
                    r.IsSuccessStatusCode 
                        ? r.Content.ReadAsStringAsync().Map(c => Success(JsonConvert.DeserializeObject<TResponse>(c))) 
                        : Task.FromResult(Error<TResponse>(new ApiError(r.Content.ToString(), r.StatusCode)))
                );
            };
        }
        
        public static Monad.Reader<HttpClient, Task<ApiResult<SearchFoodstuffResponse>>> Get(SearchFoodstuffRequest request)
        {
            return Post<SearchFoodstuffRequest, SearchFoodstuffResponse>(request, "/foodstuffs/");
        }

        public static Monad.Reader<HttpClient, Task<ApiResult<ShoppingListResponse>>> GetShoppingList()
        {
            throw new NotImplementedException();
        }

        public static Monad.Reader<HttpClient, Task<ApiResult<MyRecipesResponse>>> GetMyRecipes()
        {
            throw new NotImplementedException();
        }
    }
}
