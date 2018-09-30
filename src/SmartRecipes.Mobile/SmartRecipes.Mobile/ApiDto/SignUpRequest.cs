using Newtonsoft.Json;

namespace SmartRecipes.Mobile.ApiDto
{
    public class SignUpRequest
    {
        public SignUpRequest(string firstName, string lastName, string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        [JsonIgnore]
        public string FirstName { get; }

        [JsonIgnore]
        public string LastName { get; }

        [JsonProperty("email")]
        public string Email { get; }

        [JsonProperty("password")]
        public string Password { get; }
    }
}
