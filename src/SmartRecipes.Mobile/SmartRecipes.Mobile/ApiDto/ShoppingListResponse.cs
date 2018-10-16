using System;
using System.Collections.Generic;

namespace SmartRecipes.Mobile.ApiDto
{
    public class ShoppingListResponse
    {
        public Guid Id { get; set; }
        
        public Guid OwnerId { get; set; }
        
        public IEnumerable<Item> Items { get; set; }
        
        public IEnumerable<RecipeItem> Recipes { get; set; }

        public class Item
        {
            public Guid FoodstuffId { get; set; }
            
            public float Amount { get; set; }
        }
        
        public class RecipeItem
        {
            public Guid RecipeId { get; set; }

            public int PersonCount { get; set; }
        }
    }
}