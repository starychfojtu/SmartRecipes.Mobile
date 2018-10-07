using System;

namespace SmartRecipes.Mobile.Models
{
    public class ShoppingListItem : FoodstuffAmount, IShoppingListItem
    {
        private ShoppingListItem(Guid id, Guid shoppingListOwnerId, Guid foodstuffId, IAmount amount) : base(id, foodstuffId, amount)
        {
            ShoppingListOwnerId = shoppingListOwnerId;
        }

        public ShoppingListItem() { /* SQLite */ }

        public Guid ShoppingListOwnerId { get; set; }

        public IShoppingListItem WithAmount(IAmount amount)
        {
            return new ShoppingListItem(Id, ShoppingListOwnerId, FoodstuffId, amount);
        }

        public static IShoppingListItem Create(IAccount owner, IFoodstuff foodstuff, IAmount amount)
        {
            return new ShoppingListItem(Guid.NewGuid(), owner.Id, foodstuff.Id, amount);
        }

        public static IShoppingListItem Create(Guid id, Guid shoppingListOwnerId, Guid foodstuffId, IAmount amount)
        {
            return new ShoppingListItem(id, shoppingListOwnerId, foodstuffId, amount);
        }
    }
}
