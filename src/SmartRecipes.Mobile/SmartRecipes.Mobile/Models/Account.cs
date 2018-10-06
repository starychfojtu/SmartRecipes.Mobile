using System;
using System.Net.Mail;

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
        
        public string _Email { get; set; }
        public MailAddress Email => new MailAddress(_Email);
        
        public string _AccessToken { get; set; }
        public AccessToken AccessToken => new AccessToken(_AccessToken);

        public static IAccount Create(Guid id, MailAddress email, AccessToken accessToken)
        {
            return new Account(id, email, accessToken);
        }
    }
}
