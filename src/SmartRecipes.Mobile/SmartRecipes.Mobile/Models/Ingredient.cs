using System;

namespace SmartRecipes.Mobile.Models
{
    public class Ingredient : FoodstuffAmount, IIngredient
    {
        private Ingredient(Guid id, Guid recipeId, Guid foodstuffId, IAmount amount) : base(id, foodstuffId, amount)
        {
            RecipeId = recipeId;
        }

        public Ingredient() { /* SQLite */ }

        public Guid RecipeId { get; set; }

        public IIngredient WithAmount(IAmount amount)
        {
            return new Ingredient(Id, RecipeId, FoodstuffId, amount);
        }

        public static IIngredient Create(Guid id, Guid recipeId, Guid foodstuffId, IAmount amount)
        {
            return new Ingredient(id, recipeId, foodstuffId, amount);
        }

        public static IIngredient Create(IRecipe recipe, IFoodstuff foodstuff, IAmount amount)
        {
            return new Ingredient(Guid.NewGuid(), recipe.Id, foodstuff.Id, amount);
        }
    }
}
