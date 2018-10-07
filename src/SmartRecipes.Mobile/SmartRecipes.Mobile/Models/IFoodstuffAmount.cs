using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IFoodstuffAmount
    {
        string Id { get; }

        Guid FoodstuffId { get; }

        float Amount { get; }
    }
}
