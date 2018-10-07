using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.WriteModels;
using Xamarin.Forms;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;

namespace SmartRecipes.Mobile.ViewModels
{
    public enum EditRecipeMode
    {
        New,
        Edit
    }

    public class EditRecipeViewModel : ViewModel
    {
        private readonly Environment environment;

        public EditRecipeViewModel(Environment environment)
        {
            this.environment = environment;
            Recipe = new FormDto(
                new ValidatableObject<string>("", n => !string.IsNullOrEmpty(n), _ => RaisePropertyChanged(nameof(Recipe))),
                new ValidatableObject<int>(4, c => c > 0, _ => RaisePropertyChanged(nameof(Recipe)))
            );
            Ingredients = ImmutableDictionary.Create<IFoodstuff, IAmount>();
            Mode = EditRecipeMode.New;
        }

        public EditRecipeMode Mode { get; set; }

        public FormDto Recipe { get; set; }

        public IImmutableDictionary<IFoodstuff, IAmount> Ingredients { get; set; }

        public IEnumerable<FoodstuffAmountCellViewModel> IngredientViewModels
        {
            get { return Ingredients.Select(kvp => ToViewModel(kvp.Key, kvp.Value)); }
        }

        public async Task OpenAddIngredientDialog()
        {
            var foodstuffs = await Navigation.SelectFoodstuffDialog();
            var newFoodstuffs = foodstuffs.Where(f => !Ingredients.ContainsKey(f)).Select(f => new KeyValuePair<IFoodstuff, IAmount>(f, f.BaseAmount));
            var newIngredients = Ingredients.AddRange(newFoodstuffs);

            UpdateIngredients(newIngredients);
        }

        public Task<IOption<UserMessage>> Submit()
        {
            if (!Recipe.IsValid)
            {
                return Task.FromResult(UserMessage.Error("Cannot submit, some of the fields are invalid.").ToOption());
            }

            var submitTask = Mode == EditRecipeMode.New
                ? CreateRecipe(r => Ingredients.Select(kvp => Ingredient.Create(r, kvp.Key, kvp.Value)))
                : UpdateRecipe(r => Ingredients.Select(kvp => Ingredient.Create(r, kvp.Key, kvp.Value)));

            return submitTask
                .Bind(_ => Application.Current.MainPage.Navigation.PopAsync())
                .Map(_ => Option.Empty<UserMessage>());
        }

        public Task<Unit> CreateRecipe(Func<IRecipe, IEnumerable<IIngredient>> getIngredients)
        {
            var recipe = Models.Recipe.Create(
                CurrentAccount,
                Recipe.Name.Value,
                Recipe.ImageUrl.ToOption().Map(url => new Uri(url)),
                Recipe.PersonCount.Value,
                Recipe.Text
            );

            return MyRecipesHandler.Add(environment, recipe, getIngredients(recipe));
        }

        public Task<Unit> UpdateRecipe(Func<IRecipe, IEnumerable<IIngredient>> getIngredients)
        {
            var recipe = Models.Recipe.Create(
                Recipe.Id.Value,
                CurrentAccount.Id,
                Recipe.Name.Value,
                new Uri(Recipe.ImageUrl),
                Recipe.PersonCount.Value,
                Recipe.Text
            );

            return MyRecipesHandler.Update(environment, recipe, getIngredients(recipe));
        }

        private Task<Unit> ChangeAmount(IFoodstuff foodstuff, Func<IAmount, IAmount, IOption<IAmount>> action)
        {
            var newAmount = action(Ingredients[foodstuff], foodstuff.AmountStep).GetOrElse(foodstuff.BaseAmount);
            var newIngredients = Ingredients.SetItem(foodstuff, newAmount);

            return Task.FromResult(UpdateIngredients(newIngredients));
        }

        private Task<IOption<UserMessage>> DeleteIngredient(IFoodstuff foodstuff)
        {
            UpdateIngredients(Ingredients.Remove(foodstuff));
            return Task.FromResult(Option.Empty<UserMessage>());
        }

        private Unit UpdateIngredients(IImmutableDictionary<IFoodstuff, IAmount> newIngredients)
        {
            Ingredients = newIngredients;
            RaisePropertyChanged(nameof(Ingredients));
            RaisePropertyChanged(nameof(IngredientViewModels));
            return Unit.Value;
        }

        private FoodstuffAmountCellViewModel ToViewModel(IFoodstuff foodstuff, IAmount amount)
        {
            return new FoodstuffAmountCellViewModel(
                foodstuff,
                amount,
                Option.Empty<IAmount>(),
                () => ChangeAmount(foodstuff, (a1, a2) => Amount.Add(a1, a2)),
                () => ChangeAmount(foodstuff, (a1, a2) => Amount.Substract(a1, a2)),
                new UserAction<Unit>(_ => DeleteIngredient(foodstuff), Icon.Delete(), 1)
            );
        }

        public class FormDto
        {
            public FormDto(ValidatableObject<string> name, ValidatableObject<int> personCount)
            {
                Name = name;
                PersonCount = personCount;
            }

            public Guid? Id { get; set; }

            public ValidatableObject<string> Name { get; set; }

            public string ImageUrl { get; set; }

            public ValidatableObject<int> PersonCount { get; set; }

            public string Text { get; set; }

            public bool IsValid
            {
                get { return Name.IsValid && PersonCount.IsValid; }
            }
        }
    }
}
