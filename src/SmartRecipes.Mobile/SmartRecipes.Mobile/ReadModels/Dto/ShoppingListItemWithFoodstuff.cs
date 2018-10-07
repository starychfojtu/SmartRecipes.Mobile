using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class ShoppingListItemWithFoodstuff
    {
        public ShoppingListItemWithFoodstuff(IFoodstuff foodstuff, IShoppingListItem item)
        {
            Foodstuff = foodstuff;
            Item = item;
        }

        public IFoodstuff Foodstuff { get; }

        public IShoppingListItem Item { get; }
        
        public ShoppingListItemWithFoodstuff WithItemAmount(IShoppingListItem item)
        {
            return new ShoppingListItemWithFoodstuff(Foodstuff, item);
        }
    }
}
