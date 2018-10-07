using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IIngredient : IFoodstuffAmount
    {
        Guid RecipeId { get; }

        IIngredient WithAmount(IAmount amount);

        // IFoodstuffAmount WithRecipe(IRecipe recipe);
    }
}
