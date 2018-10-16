using System.Linq;
using Xamarin.Forms;
using SmartRecipes.Mobile.ViewModels;
using FFImageLoading.Transformations;
using FuncSharp;
using Xamarin.Forms.Internals;

namespace SmartRecipes.Mobile.Views
{
    public partial class FoodstuffAmountCell : ViewCell
    {
        public FoodstuffAmountCell()
        {
            InitializeComponent();

            Image.Transformations.Add(new CircleTransformation());

            MinusButton.Clicked += async (s, e) => await ViewModel.OnMinus.Invoke(Unit.Value);
            PlusButton.Clicked += async (s, e) => await ViewModel.OnPlus.Invoke(Unit.Value);
        }

        private FoodstuffAmountCellViewModel ViewModel => BindingContext as FoodstuffAmountCellViewModel;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            Reset();
            
            if (ViewModel != null)
            {
                var foodstuff = ViewModel.Foodstuff;
                var amountText = ViewModel.RequiredAmount.Match(
                    a => $"{ViewModel.Amount} / {a} {foodstuff.BaseAmount.Unit.ToString()}",
                    _ => foodstuff.BaseAmount.WithCount(ViewModel.Amount).ToString()
                );

                NameLabel.Text = ViewModel.Foodstuff.Name;
                AmountLabel.Text = amountText;
                MinusButton.IsVisible = ViewModel.OnMinus != null;
                Image.Source = ViewModel.Foodstuff.ImageUrl;
                
                var actions = ViewModel.MenuActions.Select(a => Controls.Controls.MenuItem(a));
                ContextActions.Clear();
                actions.ForEach(a => ContextActions.Add(a));
            }
        }
        
        private void Reset()
        {
            NameLabel.Text = "";
            AmountLabel.Text = "";
            MinusButton.IsVisible = false;
            Image.Source = null;
            ContextActions.Clear();
        }
    }
}
