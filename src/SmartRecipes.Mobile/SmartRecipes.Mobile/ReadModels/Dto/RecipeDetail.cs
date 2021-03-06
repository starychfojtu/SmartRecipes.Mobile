﻿using SmartRecipes.Mobile.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class RecipeDetail
    {
        public RecipeDetail(IRecipe recipe, IEnumerable<IngredientWithFoodstuff> ingredients)
        {
            Recipe = recipe;
            Ingredients = ingredients.Select(i => i);
        }

        public IRecipe Recipe { get; }

        public IEnumerable<IngredientWithFoodstuff> Ingredients { get; }
    }
}
