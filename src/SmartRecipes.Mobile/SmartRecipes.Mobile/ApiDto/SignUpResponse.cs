using System;
using Newtonsoft.Json;

namespace SmartRecipes.Mobile.ApiDto
{
    public class SignUpResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
