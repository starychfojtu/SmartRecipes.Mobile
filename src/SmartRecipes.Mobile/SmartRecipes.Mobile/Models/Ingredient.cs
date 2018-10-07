using System;

namespace SmartRecipes.Mobile.Models
{
    public class Ingredient : FoodstuffAmount, IIngredient
    {
        private Ingredient(Guid recipeId, Guid foodstuffId, float amount) : base(recipeId.ToString() + foodstuffId, foodstuffId, amount)
        {
            RecipeId = recipeId;
        }

        public Ingredient() { /* SQLite */ }

        public Guid RecipeId { get; set; }

        public IIngredient WithAmount(float amount)
        {
            return new Ingredient(RecipeId, FoodstuffId, amount);
        }

        public static IIngredient Create(Guid recipeId, Guid foodstuffId, float amount)
        {
            return new Ingredient(recipeId, foodstuffId, amount);
        }

        public static IIngredient Create(IRecipe recipe, IFoodstuff foodstuff, float amount)
        {
            return new Ingredient(recipe.Id, foodstuff.Id, amount);
        }
    }
}
