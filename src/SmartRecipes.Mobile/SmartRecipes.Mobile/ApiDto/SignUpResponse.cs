using System;
using Newtonsoft.Json;

namespace SmartRecipes.Mobile.ApiDto
{
    public class SignUpResponse
    {
        public static readonly string AccountAlreadyExists = "Account already exists.";
        public static readonly string InvalidEmail = "Email is invalid.";
        public static readonly string PasswordTooShort = "Password mus be at least 10 characters long.";
        
        [JsonProperty("id")]
        public Guid Id { get; set; }
        

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
