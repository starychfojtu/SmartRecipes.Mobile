using System;
using Newtonsoft.Json;

namespace SmartRecipes.Mobile.ApiDto
{
    public class SignInResponse
    {
        [JsonProperty("value")]
        public string Token { get; }

        [JsonProperty("accountId")]
        public Guid AccountId { get; }
        
        [JsonProperty("email")]
        public string Email { get; }
    }
}
