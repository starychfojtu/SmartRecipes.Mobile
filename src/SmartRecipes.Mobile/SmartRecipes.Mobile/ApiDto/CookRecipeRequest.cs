using System;
using Newtonsoft.Json;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ApiDto
{
    public class CookRecipeRequest
    {
        public CookRecipeRequest(AccessToken token, Guid recipeId)
        {
            Token = token;
            RecipeId = recipeId;
        }

        [JsonIgnore]
        public AccessToken Token { get; }
        
        [JsonProperty("recipeId")]
        public Guid RecipeId { get; }
    }
}