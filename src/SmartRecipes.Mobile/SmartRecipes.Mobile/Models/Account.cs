using System;
using System.Net.Mail;
using SQLite;

namespace SmartRecipes.Mobile.Models
{
    public class Account : IAccount
    {
        private Account(Guid id, MailAddress email, AccessToken accessToken)
        {
            Id = id;
            _Email = email.Address;
            _AccessToken = accessToken.Value;
        }
        
        public Account() { /* SqlLite */ }

        public Guid Id { get; }
        
        [Ignore]
        public MailAddress Email => new MailAddress(_Email);
        public string _Email { get; set; }
        
        [Ignore]
        public AccessToken AccessToken => new AccessToken(_AccessToken);
        public string _AccessToken { get; set; }

        public static IAccount Create(Guid id, MailAddress email, AccessToken accessToken) =>
            new Account(id, email, accessToken);
    }
}
