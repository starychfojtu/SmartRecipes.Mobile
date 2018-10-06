using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Extensions;
using Monad;
using static SmartRecipes.Mobile.Infrastructure.ApiResult;

namespace SmartRecipes.Mobile.Infrastructure
{   
    public static class ApiClient
    {
        public static Reader<Environment, Task<ApiResult<SignInResponse>>> Post(SignInRequest request)
        {
            return Post<SignInRequest, SignInResponse>(request, "/signIn");
        }
        
        public static Reader<Environment, Task<ApiResult<SignUpResponse>>> Post(SignUpRequest request)
        {
            return Post<SignUpRequest, SignUpResponse>(request, "/signUp");
        }
        
        public static Reader<Environment, Task<ApiResult<ChangeFoodstuffAmountResponse>>> Post(ChangeFoodstuffAmountRequest request)
        {
            return Post<ChangeFoodstuffAmountRequest, ChangeFoodstuffAmountResponse>(request, "/signIn");
        }
        
        public static Reader<Environment, Task<ApiResult<SearchFoodstuffResponse>>> Get(SearchFoodstuffRequest request)
        {
            return Post<SearchFoodstuffRequest, SearchFoodstuffResponse>(request, "/foodstuffs/");
        }

        public static Reader<Environment, Task<ApiResult<ShoppingListResponse>>> GetShoppingList()
        {
            throw new NotImplementedException();
        }

        public static Reader<Environment, Task<ApiResult<MyRecipesResponse>>> GetMyRecipes()
        {
            throw new NotImplementedException();
        }
        
        private static Reader<Environment, Task<ApiResult<TResponse>>> Post<TRequest, TResponse>(TRequest request, string route)
        {
            return env =>
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
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
    }
}
