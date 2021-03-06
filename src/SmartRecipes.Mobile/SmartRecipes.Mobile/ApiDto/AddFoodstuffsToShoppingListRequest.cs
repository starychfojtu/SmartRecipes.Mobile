using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ApiDto
{
    public class AddFoodstuffsToShoppingListRequest
    {
        public AddFoodstuffsToShoppingListRequest(AccessToken token, IEnumerable<Guid> foodstuffIds)
        {
            Token = token;
            FoodstuffIds = foodstuffIds;
        }

        [JsonIgnore]
        public AccessToken Token { get; }

        [JsonProperty("itemIds")]
        public IEnumerable<Guid> FoodstuffIds { get; }
    }
}