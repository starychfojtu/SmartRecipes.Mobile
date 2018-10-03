using System.ComponentModel;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        private static IAccount clientAccount;

        protected static IAccount CurrentAccount
        {
            get { return clientAccount ?? (clientAccount = FakeData.FakeAccount()); }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected Task<UserActionResult> Error(UserMessage message)
        {
            return Task.FromResult(UserActionResult.Error(message));
        }
        
        protected Task<UserActionResult> Error(string message)
        {
            return Task.FromResult(UserActionResult.Error(new UserMessage("Error", message)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual Task InitializeAsync()
        {
            return Task.FromResult(false);
        }
    }
}
