using System;
using SQLite;

namespace SmartRecipes.Mobile.Models
{
    public abstract class FoodstuffAmount : ComposedKeyEntity, IFoodstuffAmount
    {
        protected FoodstuffAmount(string id, Guid foodstuffId, float amount) : base(id)
        {
            FoodstuffId = foodstuffId;
            Amount = amount;
        }

        protected FoodstuffAmount() { /* SQLite */ }

        public Guid FoodstuffId { get; set; }
        
        public float Amount { get; set; }
    }
}
