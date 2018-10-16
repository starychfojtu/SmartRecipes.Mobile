using System;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ApiDto
{
    public class ChangeFoodstuffAmountRequest
    {
        public ChangeFoodstuffAmountRequest(AccessToken token, Guid foodstuffId, float amount)
        {
            Token = token;
            FoodstuffId = foodstuffId;
            Amount = amount;
        }

        public AccessToken Token { get; }
        
        public Guid FoodstuffId { get; }

        public float Amount { get; }
    }
}
