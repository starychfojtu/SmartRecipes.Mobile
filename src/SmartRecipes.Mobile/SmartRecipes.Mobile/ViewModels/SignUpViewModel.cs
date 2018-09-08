using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.WriteModels;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Infrastructure;

namespace SmartRecipes.Mobile.ViewModels
{
    public class SignUpViewModel : ViewModel
    {
        private readonly Enviroment enviroment;

        public SignUpViewModel(Enviroment enviroment)
        {
            this.enviroment = enviroment;
            FirstName = ValidatableObject.Create<string>(
                s => Validation.NotEmpty(s) && Validation.IsLongerThan(s, 1),
                _ => RaisePropertyChanged(nameof(Email))
            );  
            LastName = ValidatableObject.Create<string>(
                s => Validation.NotEmpty(s) && Validation.IsLongerThan(s, 1),
                _ => RaisePropertyChanged(nameof(Email))
            );
            Email = ValidatableObject.Create<string>(
                s => Validation.NotEmpty(s) && Validation.IsEmail(s),
                _ => RaisePropertyChanged(nameof(Email))
            );
            Password = ValidatableObject.Create<string>(
                s => Validation.NotEmpty(s),
                _ => RaisePropertyChanged(nameof(Password))
            );
        }

        public ValidatableObject<string> FirstName{ get; set; }

        public ValidatableObject<string> LastName { get; set; }
        
        public ValidatableObject<string> Email { get; set; }

        public ValidatableObject<string> Password { get; set; }

        public bool FormIsValid
        {
            get { return FirstName.IsValid && LastName.IsValid && Email.IsValid && Password.IsValid; }
        }

        public async Task<bool> SignUp()
        {
            if (FormIsValid)
            {
                var credentials = new SignUpCredentials(FirstName.Data, LastName.Data, Email.Data, Password.Data);
                var signUpResult = await UserHandler.SignUp(enviroment.Api, credentials);

                if (signUpResult)
                {
                    await enviroment.Db.Seed();
                    await Navigation.OpenApp();
                    return true;
                }
            }
            return false;
        }
    }
}
