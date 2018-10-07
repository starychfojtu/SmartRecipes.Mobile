using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;
using System.Collections.Generic;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class MyRecipesHandler
    {
        public static Task<Unit> Add(Environment environment, IRecipe recipe, IEnumerable<IFoodstuffAmount> ingredients)
        {
            return environment.Db.AddAsync(recipe.ToEnumerable())
                .Bind(_ => environment.Db.AddAsync(ingredients));
        }

        public static Task<Unit> Update(Environment environment, IRecipe recipe, IEnumerable<IIngredient> ingredients)
        {
            return environment.Db.UpdateAsync(recipe)
                .Bind(_ => DeleteIngredients(environment.Db, recipe))
                .Bind(_ => environment.Db.AddAsync(ingredients));
        }
        
        public static ITry<Task<Unit>> Delete(Environment environment, IRecipe recipe)
        {
            return Try.Create(unit =>
            {
                var recipeInShoppingList = environment.Db.GetTableMapping<RecipeInShoppingList>();
                var recipeInShoppingListRecipeId = recipeInShoppingList.FindColumnWithPropertyName(nameof(RecipeInShoppingList.RecipeId));
                var deleteRecipesInShoppingLists = $"DELETE FROM {recipeInShoppingList.TableName} WHERE {recipeInShoppingListRecipeId.Name} = ?";
                
                var ingredientAmounts = environment.Db.GetTableMapping<Ingredient>();
                var ingredientAmountRecipeId = recipeInShoppingList.FindColumnWithPropertyName(nameof(Ingredient.RecipeId));
                var deleteIngredientAMounts = $"DELETE FROM {ingredientAmounts.TableName} WHERE {ingredientAmountRecipeId.Name} = ?";

                return environment.Db.Execute(deleteRecipesInShoppingLists, recipe.Id)
                    .Bind(_ => environment.Db.Execute(deleteIngredientAMounts, recipe.Id))
                    .Bind(_ => environment.Db.Delete(recipe));
            });
        }

        private static Task<Unit> DeleteIngredients(Database database, IRecipe recipe)
        {
            var ingredientAmounts = database.GetTableMapping<Ingredient>();
            var recipeId = ingredientAmounts.FindColumnWithPropertyName(nameof(Ingredient.RecipeId)).Name;
            var deleteCommand = $@"DELETE FROM {ingredientAmounts.TableName} WHERE {recipeId} = ?";

            return database.Execute(deleteCommand, recipe.Id);
        }
    }
}
