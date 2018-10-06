using System.Net;
using System.Threading.Tasks;
using FuncSharp;
using Monad;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class AccountRepository
    {
        private static IOption<IAccount> Cache { get; set; }
        
        public static Reader<Environment, Task<IOption<IAccount>>> GetCurrentAccount()
        {
            return env => Cache.Match(
                isCached => Cache.Async(),
                _ => env.Db.Accounts.FirstOption<Account, IAccount>()
            );
        }
    }
}