using System.Collections.Generic;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Models;
using SQLite;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Infrastructure
{
    public sealed class Database
    {
        private const string FileName = "SmartRecies.db";

        private SQLiteAsyncConnection connection;

        public Task<Unit> AddAsync<T>(IEnumerable<T> items)
        {
            return Connection.InsertAllAsync(items).ToUnit();
        }

        public Task<Unit> UpdateAsync<T>(IEnumerable<T> items)
        {
            return Connection.UpdateAllAsync(items).ToUnit();
        }

        public Task<Unit> UpdateAsync<T>(T item)
        {
            return Connection.UpdateAsync(item).ToUnit();
        }

        public Task<Unit> AddOrReplaceAsync<T>(T item)
        {
            return Connection.InsertOrReplaceAsync(item).ToUnit();
        }

        public Task<Unit> Delete<T>(T item)
        {
            return Connection.DeleteAsync(item).ToUnit();
        }

        public Task<IEnumerable<T>> Query<T>(string sql, params object[] args)
            where T : new()
        {
            return connection.QueryAsync<T>(sql, args).Map(t => (IEnumerable<T>)t);
        }

        public Task<Unit> Execute(string sql, params object[] args)
        {
            return connection.QueryAsync<int>(sql, args).ToUnit();
        }

        public TableMapping GetTableMapping<T>()
        {
            return connection.GetConnection().GetMapping<T>();
        }

        public AsyncTableQuery<Recipe> Recipes
        {
            get { return Connection.Table<Recipe>(); }
        }

        public AsyncTableQuery<Ingredient> Ingredients
        {
            get { return Connection.Table<Ingredient>(); }
        }

        public AsyncTableQuery<ShoppingListItem> ShoppingListItems
        {
            get { return Connection.Table<ShoppingListItem>(); }
        }

        public AsyncTableQuery<Foodstuff> Foodstuffs
        {
            get { return Connection.Table<Foodstuff>(); }
        }

        public AsyncTableQuery<RecipeInShoppingList> RecipeInShoppingLists
        {
            get { return Connection.Table<RecipeInShoppingList>(); }
        }
        
        public AsyncTableQuery<Account> Accounts
        {
            get { return Connection.Table<Account>(); }
        }

        private SQLiteAsyncConnection Connection
        {
            get { return connection ?? (connection = InitializeDb()); }
        }

        private SQLiteAsyncConnection InitializeDb()
        {
            var conn = new SQLiteAsyncConnection(DependencyService.Get<IFileHelper>().GetLocalFilePath(FileName));
            var syncConn = conn.GetConnection();
            syncConn.CreateTable<Recipe>();
            syncConn.CreateTable<Ingredient>();
            syncConn.CreateTable<ShoppingListItem>();
            syncConn.CreateTable<Foodstuff>();
            syncConn.CreateTable<RecipeInShoppingList>();
            syncConn.CreateTable<Account>();

            return conn;
        }
    }
}
