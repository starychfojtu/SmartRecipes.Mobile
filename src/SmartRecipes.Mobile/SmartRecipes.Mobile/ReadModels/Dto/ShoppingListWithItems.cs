using System.Collections.Generic;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class ShoppingListWithItems
    {
        public ShoppingListWithItems(IShoppingList shoppingList, IEnumerable<IShoppingListItem> items, IEnumerable<IShoppingListRecipeItem> recipeItems)
        {
            ShoppingList = shoppingList;
            Items = items;
            RecipeItems = recipeItems;
        }
        
        public IShoppingList ShoppingList { get; }
        
        public IEnumerable<IShoppingListItem> Items { get; }
        
        public IEnumerable<IShoppingListRecipeItem> RecipeItems { get; }
    }
}