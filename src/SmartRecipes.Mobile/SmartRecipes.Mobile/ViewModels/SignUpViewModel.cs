using System;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.WriteModels.Dto;

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
            if (FormIsValid)
            {
                var parameters = new SignUpParameters(new Credentials(Email.Value, Password.Value));
                
            }

            return Task.FromResult(UserActionResult.Error(UserMessages.InvalidForm()));
        }
        
        private bool FormIsValid
        {
            get { return Email.IsValid && Password.IsValid; }
        }
    }
}
