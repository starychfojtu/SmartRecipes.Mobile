﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SmartRecipes.Mobile.Models;
using Xamarin.Forms;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Views
{
    public partial class RecipeCell : ViewCell
    {
        private IImmutableList<Button> actionButtons;
        
        public RecipeCell()
        {
            actionButtons = ImmutableList.Create<Button>();
            InitializeComponent();
        }

        private RecipeCellViewModel ViewModel => BindingContext as RecipeCellViewModel;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ViewModel != null)
            {
                var actions = ViewModel.Actions.OrderBy(a => a.Order).ToImmutableList();
                var allActions = actions.Add(new UserAction<IRecipe>(r => ViewModel.EditRecipe(), Icon.Edit(), int.MaxValue));
                var newActionButtons = allActions.Select(a =>
                {
                    var actionButton = new Button
                    {
                        HeightRequest = 64,
                        WidthRequest = 64,
                        Image = a.Icon.ImageName,
                        VerticalOptions = LayoutOptions.Center,
                        BackgroundColor = Color.Transparent
                    };
                    return actionButton.Tee(b => b.Clicked += async (s, e) => await a.Action(ViewModel.Recipe));
                });

                NameLabel.Text = ViewModel.Recipe.Name;
                ReplaceActions(newActionButtons);
                
                // TODO: in future versions
                // IngredientsStackLayout.Children.Clear();
                //var thumbnails = ingredients.Select(i => Image.Thumbnail(i.Foodstuff.ImageUrl));
                //IngredientsStackLayout.Children.AddRange(thumbnails);
            }
        }

        private void ReplaceActions(IEnumerable<Button> buttons)
        {
            actionButtons.Iter(b => ActionContainer.Children.Remove(b));
            actionButtons = buttons.ToImmutableList();
            ActionContainer.Children.AddRange(actionButtons);
        }
    }
}
