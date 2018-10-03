using System;
using System.Net.Http;

namespace SmartRecipes.Mobile.Infrastructure
{
    public class Environment
    {
        public Environment(HttpClient httpClient, Database database)
        {
            HttpClient = httpClient;
            Db = database;
            HttpClient.BaseAddress = new Uri("https://smart-recipes.herokuapp.com");
        }

        public HttpClient HttpClient { get; }

        public Database Db { get; }
    }
}
