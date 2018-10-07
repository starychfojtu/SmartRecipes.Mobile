using System;

namespace SmartRecipes.Mobile.Models
{
    public sealed class ShoppingListRecipeItem : Entity, IShoppingListRecipeItem
    {
        private ShoppingListRecipeItem(Guid recipeId, Guid shoppingListId, int personCount) : base(id)
        {
            RecipeId = recipeId;
            ShoppingListId = shoppingListId;
            PersonCount = personCount;
        }

        public ShoppingListRecipeItem() : base(Guid.Empty) { /* sqlite */ }

        public Guid RecipeId { get; set; }

        public Guid ShoppingListId { get; set; }

        public int PersonCount { get; set; }
        
        public IShoppingListRecipeItem AddPersons(int count)
        {
            return new ShoppingListRecipeItem(,RecipeId, ShoppingListId, PersonCount + count);
        }

        public static IShoppingListRecipeItem Create(Guid shoppingListId, Guid recipeId,, int personCount)
        {
            return new ShoppingListRecipeItem(,recipeId, shoppingListId, personCount);
        }
    }
}
