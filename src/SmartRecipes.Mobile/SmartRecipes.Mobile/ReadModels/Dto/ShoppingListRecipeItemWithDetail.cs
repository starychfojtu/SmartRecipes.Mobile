using SmartRecipes.Mobile.Models;
namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class ShoppingListRecipeItemWithDetail
    {
        public ShoppingListRecipeItemWithDetail(RecipeDetail detail, IShoppingListRecipeItem shoppingListRecipeItem)
        {
            Detail = detail;
            ShoppingListRecipeItem = shoppingListRecipeItem;
        }

        public IShoppingListRecipeItem ShoppingListRecipeItem { get; }

        public RecipeDetail Detail { get; }
    }
}
