using System.Collections.Generic;
using FuncSharp;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.ReadModels.Dto;

namespace SmartRecipes.Mobile.ViewModels
{
    public class RecipeCellViewModel
    {
        public RecipeCellViewModel(RecipeDetail detail, IOption<int> personCount, params UserAction<IRecipe>[] actions)
        {
            Detail = detail;
            Actions = actions;
            PersonCount = personCount.GetOrElse(detail.Recipe.PersonCount);
        }

        public RecipeDetail Detail { get; }
        
        public IEnumerable<UserAction<IRecipe>> Actions { get; }

        public int PersonCount { get; }
    }
}
