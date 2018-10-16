using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IShoppingListRecipeItem
    {
        string Id { get; }

        Guid RecipeId { get; }

        Guid ShoppingListId { get; }

        int PersonCount { get; }

        IShoppingListRecipeItem AddPersons(int count);
    }
}
