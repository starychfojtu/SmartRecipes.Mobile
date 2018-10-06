using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FuncSharp;
using SQLite;

namespace SmartRecipes.Mobile.Extensions
{
    public static class SqlLiteExtensions
    {
        public static Task<IEnumerable<T>> ToEnumerableAsync<T>(this AsyncTableQuery<T> tableQuery)
            where T : new()
        {
            return tableQuery.ToListAsync().Map(t => (IEnumerable<T>)t);
        }

        public static Task<IEnumerable<U>> ToEnumerableAsync<T, U>(this AsyncTableQuery<T> tableQuery)
            where T : class, new()
            where U : class
        {
            return tableQuery.ToEnumerableAsync().Map(t => t.Select(i => i as U));
        }

        public static Task<IOption<B>> FirstOption<A, B>(this AsyncTableQuery<A> tableQuery)
            where A : new()
            where B : class
        {
            return tableQuery.FirstOrDefaultAsync().Map(a => a.As<B>());
        }
    }
}
