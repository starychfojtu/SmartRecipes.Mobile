using System;
using SQLite;

namespace SmartRecipes.Mobile.Models
{
    public abstract class FoodstuffAmount : IFoodstuffAmount
    {
        protected FoodstuffAmount(string id, Guid foodstuffId, float amount)
        {
            Id = id;
            FoodstuffId = foodstuffId;
            Amount = amount;
        }

        protected FoodstuffAmount() { /* SQLite */ }

        [PrimaryKey]
        public string Id { get; set; }

        public Guid FoodstuffId { get; set; }
        
        public float Amount { get; set; }
    }
}
