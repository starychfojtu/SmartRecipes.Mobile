using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.Infrastructure;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class Repository
    {
        public static Monad.Reader<Enviroment, Task<TModel>> RetrievalAction<TModel, TResponse>(
            Func<Unit, Monad.Reader<HttpClient, Task<Either<TResponse, ApiError>>>> apiCall,
            Monad.Reader<Enviroment, Task<TModel>> databaseQuery,
            Func<TResponse, TModel> responseMapper,
            Func<TModel, IEnumerable<object>> envtabaseMapper)
        {
            return env => (apiCall(Unit.Default)(env.HttpClient)).Bind(response => response.Match(
                e => databaseQuery(env),
                r =>
                {
                    var model = responseMapper(r);
                    var newItems = envtabaseMapper(model);
                    var updateTask = newItems.Fold(Task.FromResult(Unit.Default), (t, i) => t.Bind(_ => env.Db.AddOrReplaceAsync(i)));
                    return updateTask.Map(_ => model);
                }
            ));
        }
    }
}
