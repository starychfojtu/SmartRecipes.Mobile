using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IIngredient : IFoodstuffAmount
    {
        Guid RecipeId { get; }
    
        IIngredient WithAmount(float amount);

        // IFoodstuffAmount WithRecipe(IRecipe shoppingListRecipe);
    }
}
