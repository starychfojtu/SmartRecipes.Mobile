﻿using Xamarin.Forms;
using SmartRecipes.Mobile.ViewModels;
using FFImageLoading.Transformations;

namespace SmartRecipes.Mobile.Views
{
    public partial class FoodstuffSearchCell : ViewCell
    {
        public FoodstuffSearchCell()
        {
            InitializeComponent();

            Image.Transformations.Add(new CircleTransformation());

            PlusButton.Clicked += (s, e) => ViewModel.OnSelected();
        }

        private FoodstuffSearchCellViewModel ViewModel => (BindingContext as FoodstuffSearchCellViewModel);

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            Reset();

            if (ViewModel != null)
            {
                NameLabel.Text = ViewModel.Foodstuff.Name;
                AmountLabel.Text = ViewModel.Foodstuff.BaseAmount.ToString();
                Image.Source = ViewModel.Foodstuff.ImageUrl;
            }
        }
        
        private void Reset()
        {
            NameLabel.Text = "";
            AmountLabel.Text = "";
            Image.Source = "";
        }
    }
}
