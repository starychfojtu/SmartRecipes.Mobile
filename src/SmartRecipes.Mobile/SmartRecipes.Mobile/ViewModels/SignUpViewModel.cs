using System;
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
            Email = ValidatableObject.Create<string>(
                s => Validation.NotEmpty(s) && Validation.IsEmail(s),
                _ => RaisePropertyChanged(nameof(Email))
            );
            Password = ValidatableObject.Create<string>(
                s => Validation.NotEmpty(s),
                _ => RaisePropertyChanged(nameof(Password))
            );
        }

        public ValidatableObject<string> Email { get; set; }

        public ValidatableObject<string> Password { get; set; }
       
        public Task<UserActionResult> SignUp()
        {
            throw new NotImplementedException();
        }
        
        private bool FormIsValid
        {
            get { return Email.IsValid && Password.IsValid; }
        }
    }
}
