using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Models;
using System.Linq;
using System;
using Monad;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class FoodstuffRepository
    {
        // Search
        
        public static Reader<Environment, Task<IEnumerable<IFoodstuff>>> Search(string query) =>
            Repository.RetrievalAction(
                ApiClient.Get(new SearchFoodstuffRequest(query)),
                SearchDb(query),
                response => ToFoodstuffs(response),
                foodstuffs => foodstuffs
            );
        
        private static Reader<Environment, Task<IEnumerable<IFoodstuff>>> SearchDb(string searchQuery) =>
            env => env.Db.GetTableMapping<Foodstuff>()
                .Pipe(mapping => (tableName: mapping.TableName, name: mapping.FindColumnWithPropertyName(nameof(Foodstuff.Name)).Name))
                .Pipe(foodstuffs => $@"SELECT * FROM {foodstuffs.tableName} WHERE LOWER({foodstuffs.name}) LIKE ?")
                .Pipe(query => env.Db.Query<Foodstuff>(query, $"%{searchQuery}%"))
                .Map(fs => fs.Select(f => f as IFoodstuff));
        
        private static IEnumerable<IFoodstuff> ToFoodstuffs(SearchFoodstuffResponse response) =>
            response.Foodstuffs.Select(f => Foodstuff.Create(f.Id, f.Name, f.ImageUrl, f.BaseAmount, f.AmountStep));
        
        // Get by ids

        public static Reader<Environment, Task<IEnumerable<IFoodstuff>>> Get(IEnumerable<Guid> ids) =>
            env => env.Db.Foodstuffs.Where(f => ids.Contains(f.Id)).ToEnumerableAsync<Foodstuff, IFoodstuff>();
    }
}
