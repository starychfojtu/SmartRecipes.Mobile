using System;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Models;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;

namespace SmartRecipes.Mobile.ViewModels
{
    public class SignUpViewModel : ViewModel
    {
        private readonly Environment environment;

        public SignUpViewModel(Environment environment)
        {
            this.environment = environment;
            Email = ValidatableObject.Create<string>(
                s => Mail.Create(s).IsSuccess,
                _ => RaisePropertyChanged(nameof(Email))
            );
            Password = ValidatableObject.Create<string>(
                s => Models.Password.Create(s).IsSuccess,
                _ => RaisePropertyChanged(nameof(Password))
            );
        }

        public ValidatableObject<string> Email { get; set; }

        public ValidatableObject<string> Password { get; set; }
       
        public Task<UserActionResult> SignUp()
        {
            throw new NotImplementedException();
        }
    }
}
