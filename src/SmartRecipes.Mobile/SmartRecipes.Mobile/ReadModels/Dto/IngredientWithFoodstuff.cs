using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class IngredientWithFoodstuff
    {
        public IngredientWithFoodstuff(IFoodstuff foodstuff, IIngredient ingredient)
        {
            Foodstuff = foodstuff;
            Ingredient = ingredient;
        }

        public IFoodstuff Foodstuff { get; }

        public IIngredient Ingredient { get; }
    }
}
