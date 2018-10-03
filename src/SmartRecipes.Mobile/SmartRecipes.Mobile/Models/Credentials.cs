using System.Net.Mail;

namespace SmartRecipes.Mobile.Models
{
    public class Credentials
    {
        public Credentials(MailAddress email, Password password)
        {
            Email = email;
            Password = password;
        }

        public MailAddress Email { get; }

        public Password Password { get; }

        public static Credentials Create(MailAddress email, Password password)
        {
            return new Credentials(email, password);
        }
    }
}
