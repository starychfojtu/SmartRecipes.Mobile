using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class Repository
    {
        public static Monad.Reader<Enviroment, Task<TModel>> RetrievalAction<TModel, TResponse>(
            Monad.Reader<Enviroment, Task<ApiResult<TResponse>>> apiCall,
            Monad.Reader<Enviroment, Task<TModel>> databaseQuery,
            Func<TResponse, TModel> responseMapper,
            Func<TModel, IEnumerable<object>> envtabaseMapper)
        {
            return env => apiCall(env).Bind(response => response.Match(
                successResponse =>
                {
                    var model = responseMapper(successResponse);
                    var newItems = envtabaseMapper(model);
                    var updateTask = newItems.Aggregate(Tasks.Unit(), (t, i) => t.Bind(_ => env.Db.AddOrReplaceAsync(i)));
                    return updateTask.Map(_ => model);
                },
                error => throw new InvalidOperationException(error.Message),
                noConnection => databaseQuery(env)
            ));
        }
    }
}
