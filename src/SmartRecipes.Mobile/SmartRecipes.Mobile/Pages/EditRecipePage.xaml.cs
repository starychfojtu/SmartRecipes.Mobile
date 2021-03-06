﻿using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.ViewModels;
using SmartRecipes.Mobile.Views;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Pages
{
    public partial class EditRecipePage : ContentPage
    {
        public EditRecipePage(EditRecipeViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

//            viewModel.BindErrors(NameEntry, vm => vm.Recipe.Name.IsValid);
//            viewModel.BindText(NameEntry, vm => vm.Recipe.Name.Value);
//            viewModel.BindText(ImageUrlEntry, vm => vm.Recipe.ImageUrl);
//            viewModel.BindErrors(PersonCountEntry, vm => vm.Recipe.PersonCount.IsValid);
//            viewModel.BindText(PersonCountEntry, vm => vm.Recipe.PersonCount.Value);
//            viewModel.BindValue(TextEditor, Editor.TextProperty, vm => vm.Recipe.Text);
//
//            IngredientsListView.ItemTemplate = new DataTemplate<FoodstuffAmountCell>();
//            viewModel.BindValue(IngredientsListView, ItemsView<Cell>.ItemsSourceProperty, vm => vm.IngredientViewModels);
//
//            AddIngredientButton.Clicked += async (s, e) => await viewModel.OpenAddIngredientDialog();
//            SubmitButton.Clicked += async (s, e) => await UserMessage.PopupAction(_ => viewModel.Submit());
        }
    }
}
