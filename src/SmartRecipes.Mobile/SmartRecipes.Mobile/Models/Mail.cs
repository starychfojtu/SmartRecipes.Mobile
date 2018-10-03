using System.Net.Mail;
using FuncSharp;

namespace SmartRecipes.Mobile.Models
{
    public static class Mail
    {
        public static ITry<MailAddress> Create(string value)
        {
            return Try.Create(_ => new MailAddress(value));
        }
    }
}