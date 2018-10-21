using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.ReadModels.Dto;
using System;
using Monad;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;
using Ingredient = SmartRecipes.Mobile.Models.Ingredient;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class RecipeRepository
    {
        // Get details by ids 

        public static Reader<Environment, Task<IEnumerable<RecipeDetail>>> GetDetails(IEnumerable<Guid> recipeIds) =>
            Get(recipeIds).Bind(recipes => GetDetails(recipes));

// Possible alternative with LINQ
//        public static Reader<Environment, Task<IEnumerable<RecipeDetail>>> GetDetails(IEnumerable<Guid> recipeIds) =>
//            from recipes in Get(recipeIds)
//            from details in GetDetails(recipes)
//            select details;
        
        // Deprecated methods
        
        // Get my recipes
        
        public static Reader<Environment, Task<IEnumerable<IRecipe>>> GetMyRecipes() =>
            Repository.RetrievalAction(
                ApiClient.GetMyRecipes(),
                env => env.Db.Recipes.ToEnumerableAsync<Recipe, IRecipe>(),
                response => response.Recipes.Select(r => ToRecipe(r)),
                recipes => recipes
            );
        
        private static Recipe ToRecipe(MyRecipesResponse.Recipe r) =>
            Recipe.Create(r.Id, r.OwnerId, r.Name, r.ImageUrl, r.PersonCount, r.Text);
        
        // Get detail(s)
        
        public static Reader<Environment, Task<IEnumerable<RecipeDetail>>> GetMyRecipeDetails() =>
            GetMyRecipes().Bind(GetDetails);

        public static Reader<Environment, Task<RecipeDetail>> GetDetail(IRecipe recipe) =>
            GetIngredients(recipe).Select(i => new RecipeDetail(recipe, i));

        public static Reader<Environment, Task<IEnumerable<RecipeDetail>>> GetDetails(IEnumerable<IRecipe> recipes) =>
            recipes.Select(GetDetail).Traverse();

        // Get by ids
        
        public static Reader<Environment, Task<IEnumerable<IRecipe>>> Get(IEnumerable<Guid> ids) =>
            env => env.Db.Recipes
                .Where(r => ids.Contains(r.Id))
                .ToEnumerableAsync<Recipe, IRecipe>();

        public static Reader<Environment, Task<IEnumerable<IngredientWithFoodstuff>>> GetIngredients(IRecipe recipe) =>
            from ingredients in GetIngredientAmounts(recipe)
            from foodstuffs in FoodstuffRepository.Get(ingredients.Select(i => i.FoodstuffId))
            select ingredients.Join(foodstuffs, i => i.FoodstuffId, f => f.Id, (i, f) => new IngredientWithFoodstuff(f, i));

        public static Reader<Environment, Task<IEnumerable<IIngredient>>> GetIngredientAmounts(IRecipe recipe) =>
            env => env.Db.Ingredients
                .Where(i => i.RecipeId == recipe.Id)
                .ToEnumerableAsync<Ingredient, IIngredient>();

        
    }
}
