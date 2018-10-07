using System.ComponentModel;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        protected IAccount CurrentAccount => null;

        protected Unit RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return Unit.Value;
        }
            
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual Task<Unit> InitializeAsync() =>
            Unit.Value.Async();
    }
}
