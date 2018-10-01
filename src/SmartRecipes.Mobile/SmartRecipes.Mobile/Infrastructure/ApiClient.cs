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
        public static Reader<Enviroment, Task<ApiResult<SignInResponse>>> Post(SignInRequest request)
        {
            return Post<SignInRequest, SignInResponse>(request, "/signIn");
        }
        
        public static Reader<Enviroment, Task<ApiResult<SignUpResponse>>> Post(SignUpRequest request)
        {
            return Post<SignUpRequest, SignUpResponse>(request, "/signUp");
        }
        
        public static Reader<Enviroment, Task<ApiResult<ChangeFoodstuffAmountResponse>>> Post(ChangeFoodstuffAmountRequest request)
        {
            return Post<ChangeFoodstuffAmountRequest, ChangeFoodstuffAmountResponse>(request, "/signIn");
        }
        
        private static Reader<Enviroment, Task<ApiResult<TResponse>>> Post<TRequest, TResponse>(TRequest request, string route)
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
                        : Error<TResponse>(new ApiError(c, r.StatusCode))
                    );
                });
            };
        }
        
        public static Reader<Enviroment, Task<ApiResult<SearchFoodstuffResponse>>> Get(SearchFoodstuffRequest request)
        {
            return Post<SearchFoodstuffRequest, SearchFoodstuffResponse>(request, "/foodstuffs/");
        }

        public static Reader<Enviroment, Task<ApiResult<ShoppingListResponse>>> GetShoppingList()
        {
            throw new NotImplementedException();
        }

        public static Reader<Enviroment, Task<ApiResult<MyRecipesResponse>>> GetMyRecipes()
        {
            throw new NotImplementedException();
        }
    }
}
