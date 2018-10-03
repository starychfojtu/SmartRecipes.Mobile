using System;
using System.Net.Mail;

namespace SmartRecipes.Mobile.Models
{
    public class Account : IAccount
    {
        public Account(Guid id, MailAddress email, AccessToken accessToken)
        {
            Id = id;
            Email = email;
            AccessToken = accessToken;
        }

        public Guid Id { get; }

        public MailAddress Email { get; }
        
        public AccessToken AccessToken { get; }
    }
}
