using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LanguageExt;
using SmartRecipes.Mobile.ApiDto;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.Infrastructure
{   
    public static class ApiClient
    {
        public static Monad.Reader<HttpClient, Task<Either<SignInResponse, ApiError>>> Post(SignInRequest request)
        {
            return Post<SignInRequest, SignInResponse>(request, "/signIn");
        }
        
        public static Monad.Reader<HttpClient, Task<Either<SignUpResponse, ApiError>>> Post(SignUpRequest request)
        {
            return Post<SignUpRequest, SignUpResponse>(request, "/signUp");
        }
        
        public static Monad.Reader<HttpClient, Task<Either<ChangeFoodstuffAmountResponse, ApiError>>> Post(ChangeFoodstuffAmountRequest request)
        {
            return Post<ChangeFoodstuffAmountRequest, ChangeFoodstuffAmountResponse>(request, "/signIn");
        }
        
        private static Monad.Reader<HttpClient, Task<Either<TResponse, ApiError>>> Post<TRequest, TResponse>(TRequest request, string route)
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
        
        public static Monad.Reader<HttpClient, Task<Either<SearchFoodstuffResponse, ApiError>>> Get(SearchFoodstuffRequest request)
        {
            return Post<SearchFoodstuffRequest, SearchFoodstuffResponse>(request, "/foodstuffs/");
        }

        public static Monad.Reader<HttpClient, Task<Either<ShoppingListResponse, ApiError>>> GetShoppingList()
        {
            throw new NotImplementedException();
        }

        public static Monad.Reader<HttpClient, Task<Option<MyRecipesResponse>>> GetMyRecipes()
        {
            throw new NotImplementedException();
        }
        
        private static Either<T, ApiError> Success<T>(T value)
        {
            return Left(value);
        }
        
        private static Either<T, ApiError> Error<T>(ApiError error)
        {
            return Right(error);
        }
    }
}
