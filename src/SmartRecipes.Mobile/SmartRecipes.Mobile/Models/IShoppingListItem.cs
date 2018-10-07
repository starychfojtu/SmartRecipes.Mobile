using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IShoppingListItem : IFoodstuffAmount
    {
        Guid ShoppingListOwnerId { get; }

        IShoppingListItem WithAmount(IAmount amount);
    }
}
