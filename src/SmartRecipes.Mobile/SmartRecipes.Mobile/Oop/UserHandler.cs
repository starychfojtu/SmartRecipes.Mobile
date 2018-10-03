using System.Net.Mail;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.Oop
{
    public class UserHandler
    {
        private readonly ApiClient client;

        public UserHandler(ApiClient client)
        {
            this.client = client;
        }
        
        public async Task<IAccount> SignIn(string email, string password)
        {
            // Contract.RequiresEmail(email)
            // Contract.RequiresPassword(password)
            var request = new SignInRequest(email, password);
            var response = await client.Post(request);
            return ToAccount(response, email);
        }

        private static IAccount ToAccount(SignInResponse response, string email)
        {
            return new Account(response.AccountId, new MailAddress(email), new AccessToken(response.Token));
        }
    }
}
