using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using System;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.ReadModels.Dto;
using System.Collections.Immutable;
using FuncSharp;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;
using static SmartRecipes.Mobile.WriteModels.ShoppingListHandler;

namespace SmartRecipes.Mobile.ViewModels
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        private readonly Environment environment;

        private ShoppingListWithItems ShoppingListWithItems { get; set; }
        
        private IImmutableDictionary<Guid, IFoodstuff> foodstuffs { get; set; }

        private RequiredAmounts requiredAmounts { get; set; }

        public ShoppingListItemsViewModel(Environment environment)
        {
            this.environment = environment;
        }

        public IEnumerable<FoodstuffAmountCellViewModel> ShoppingListItems =>
            ShoppingListWithItems.Items.Select(i => ToViewModel(i));
        
        // Initialize

        public override Task<Unit> InitializeAsync() =>
            UpdateState();
        
        // Refresh

        public Task Refresh() =>
            InitializeAsync();
        
        // Open add foodstuff dialog
        
        public Task<Unit> OpenAddFoodstuffDialog() =>
            Navigation
                .SelectFoodstuffDialog()
                .Bind(selected => ShoppingListHandler.Add(CurrentAccount, selected).Execute(environment))
                .Map(result => result.Success.Get())
                .Bind(shoppingList => UpdateState());
        
        // Delete item

        private Task<IOption<UserMessage>> Remove(IShoppingListItem item) =>
            ShoppingListHandler
                .Remove(CurrentAccount, foodstuffs.Get(item.FoodstuffId).Get().ToEnumerable())
                .Map(result => ProcessResult(result))
                .Execute(environment);
        
        private IOption<UserMessage> ProcessResult(ITry<ShoppingListWithItems, RemoveFoodstuffsError> result) =>
            result.Match(
                s => UpdateState().Pipe(_ => Option.Empty<UserMessage>()),
                e => e.Match(
                    RemoveFoodstuffsError.FoodstuffIsRequiredInRecipe, _ => UserMessages.FoodstuffRequiredInRecipe().ToOption(),
                    RemoveFoodstuffsError.FoodstuffNotInShoppingList, _ => throw new InvalidOperationException()
                )
            );

        // Change Amount
        
        private Task<IOption<UserMessage>> ChangeAmount(IShoppingListItem item, float newAmount) =>
            ShoppingListHandler
                .ChangeAmount(CurrentAccount, foodstuffs.Get(item.FoodstuffId).Get(), newAmount)
                .Map(result => ProcessResult(result))
                .Execute(environment);

        private IOption<UserMessage> ProcessResult(ITry<ShoppingListWithItems, ChangeAmountError> result) =>
            UpdateShoppingList(result.Success.Get())
                .Pipe(_ => Option.Empty<UserMessage>());
        
        // Shared

        private FoodstuffAmountCellViewModel ToViewModel(IShoppingListItem item) =>
            ToViewModel(item, foodstuffs.Get(item.FoodstuffId).Get());
            
        private FoodstuffAmountCellViewModel ToViewModel(IShoppingListItem item, IFoodstuff foodstuff) =>
            new FoodstuffAmountCellViewModel(
                foodstuff,
                item.Amount,
                requiredAmounts.Get(foodstuff),
                _ => ChangeAmount(item, item.Amount + foodstuff.AmountStep.Count),
                _ => ChangeAmount(item, Math.Max(0, item.Amount - foodstuff.AmountStep.Count)),
                new UserAction<Unit>(_ => Remove(item), Icon.Delete(), order: 1)
            );
        
        private Task<Unit> UpdateState() =>
            ShoppingListRepository.GetRequiredAmounts(CurrentAccount)
                .Map(amounts => requiredAmounts = amounts)
                .Bind((RequiredAmounts _) => ShoppingListRepository.GetWithItems(CurrentAccount))
                .Map(shoppingList => UpdateShoppingList(shoppingList))
                .Bind(shoppingList => FoodstuffRepository.Get(shoppingList.Items.Select(i => i.FoodstuffId)))
                .Map(fs => foodstuffs = fs.ToImmutableDictionary(f => f.Id))
                .Map(_ => Unit.Value)
                .Execute(environment);

// Possible LINQ version
//        private Monad.Reader<Environment, Task<Unit>> UpdateStateLINQVersion() =>
//            from requiredAmounts in ShoppingListRepository.GetRequiredAmounts(CurrentAccount)
//            from shoppingList in ShoppingListRepository.GetWithItems(CurrentAccount)
//            let foodstuffIds = shoppingList.Items.Select(i => i.FoodstuffId)
//            from foodstuffs in FoodstuffRepository.Get(foodstuffIds)
//            let _1 = UpdateShoppingList(shoppingList)
//            let _2 = this.foodstuffs = foodstuffs.ToImmutableDictionary(f => f.Id)
//            let _3 = this.requiredAmounts = requiredAmounts
//            select Unit.Value;
        
        private ShoppingListWithItems UpdateShoppingList(ShoppingListWithItems shoppingList) =>
            RaisePropertyChanged(nameof(ShoppingListItems))
                .Pipe(_ => ShoppingListWithItems = shoppingList);
    }
}
