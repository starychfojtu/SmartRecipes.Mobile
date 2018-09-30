using Newtonsoft.Json;

namespace SmartRecipes.Mobile.ApiDto
{
    public class SignInRequest
    {
        public SignInRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }

        [JsonProperty("email")]
        public string Email { get; }

        [JsonProperty("password")]
        public string Password { get; }
    }
}
