using System;

namespace SmartRecipes.Mobile.Models
{
    public class Account : IAccount
    {
        public Account(Guid id, string email, AccessToken accessToken)
        {
            Id = id;
            Email = email;
            AccessToken = accessToken;
        }

        public Guid Id { get; }

        public string Email { get; }
        
        public AccessToken AccessToken { get; }
    }
}
