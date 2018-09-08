using System;

namespace SmartRecipes.Mobile.ApiDto
{
    public class SignUpResponse
    {
        public SignUpResponse(Account account)
        {
            NewAccount = account;
        }

        public Account NewAccount { get; }

        public class Account
        {
            public Account(string email, Guid id)
            {
                Email = email;
                Id = id;
            }

            public string Email { get; }
            
            public Guid Id { get; }
        }
    }
}
