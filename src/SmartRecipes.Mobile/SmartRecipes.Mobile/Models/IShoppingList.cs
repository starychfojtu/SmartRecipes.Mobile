using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IShoppingList
    {
        Guid Id { get; }
        
        Guid OwnerId { get; }
    }

    public sealed class ShoppingList : IShoppingList
    {
        private ShoppingList(Guid id, Guid ownerId)
        {
            Id = id;
            OwnerId = ownerId;
        }

        public ShoppingList() { /* SqlLite */ }
        
        public Guid Id { get; set; }
        
        public Guid OwnerId { get; set; }

        public static ShoppingList Create(Guid id, Guid ownerId) =>
            new ShoppingList(id, ownerId);
    }
}