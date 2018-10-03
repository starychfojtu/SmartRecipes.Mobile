using System;
using System.Net.Mail;

namespace SmartRecipes.Mobile.Models
{
    public interface IAccount
    {
        Guid Id { get; }

        MailAddress Email { get; }
        
        AccessToken AccessToken { get; }
    }
}
