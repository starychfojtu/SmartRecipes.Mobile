using System;

namespace SmartRecipes.Mobile.Models
{
    public class ShoppingListItem : FoodstuffAmount, IShoppingListItem
    {
        private ShoppingListItem(Guid shoppingListId, Guid foodstuffId, float amount)
            : base(shoppingListId.ToString() + foodstuffId, foodstuffId, amount)
        {
            ShoppingListId = shoppingListId;
        }

        public ShoppingListItem() { /* SQLite */ }

        public Guid ShoppingListId { get; set; }

        public IShoppingListItem WithAmount(float amount)
        {
            return new ShoppingListItem(ShoppingListId, FoodstuffId, amount);
        }

        public static IShoppingListItem Create(Guid shoppingListId, Guid foodstuffId, float amount)
        {
            return new ShoppingListItem(shoppingListId, foodstuffId, amount);
        }
    }
}
