using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IShoppingListItem : IFoodstuffAmount
    {
        Guid ShoppingListId { get; }    

        IShoppingListItem WithAmount(float amount);
    }
}
