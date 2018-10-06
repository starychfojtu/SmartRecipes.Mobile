using System.ComponentModel;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual Task InitializeAsync()
        {
            return Task.FromResult(false);
        }
    }
}
