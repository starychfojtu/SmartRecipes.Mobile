using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IShoppingListRecipeItem
    {
        Guid Id { get; }

        Guid RecipeId { get; }

        Guid ShoppingListId { get; }

        int PersonCount { get; }

        IShoppingListRecipeItem AddPersons(int count);
    }
}
