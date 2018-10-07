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

namespace SmartRecipes.Mobile.ViewModels
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        private readonly Environment environment;

        private IImmutableList<ShoppingListItem> shoppingListItems { get; set; }

        private IImmutableDictionary<IFoodstuff, IAmount> requiredAmounts { get; set; }

        public ShoppingListItemsViewModel(Environment environment)
        {
            this.environment = environment;
            shoppingListItems = ImmutableList.Create<ShoppingListItem>();
        }

        public IEnumerable<FoodstuffAmountCellViewModel> ShoppingListItems =>
            shoppingListItems.Select(i => ToViewModel(i));
        
        // Initialize

        public override Task<Unit> InitializeAsync() =>
            ShoppingListRepository.GetRequiredAmounts(CurrentAccount)
                .Map(amounts => requiredAmounts = amounts)
                .Bind((IImmutableDictionary<IFoodstuff, IAmount> _) => ShoppingListRepository.GetItems(CurrentAccount.Id))
                .Map(items => UpdateShoppingListItems(items))
                .Execute(environment);
        
        // Refresh

        public Task Refresh() =>
            InitializeAsync();
        
        // Open add foodstuff dialog
        
        public Task<Unit> OpenAddFoodstuffDialog() =>
            Navigation
                .SelectFoodstuffDialog()
                .Bind(selected => ShoppingListHandler.AddToShoppingList(environment, CurrentAccount, selected))
                .Map(newItems => shoppingListItems.Concat(newItems))
                .Map(allItems => UpdateShoppingListItems(allItems));
        
        // Item action
        
        private Task<Unit> ShoppingListItemAction(ShoppingListItem shoppingListItem, Func<ShoppingListItem, IShoppingListItemAmount> action) =>
            action(shoppingListItem)
                .Pipe(newAmount => shoppingListItem.WithItemAmount(newAmount))
                .Pipe(newItem => ShoppingListHandler.Update(environment, newItem.ItemAmount).Map(_ => newItem))
                .Map(newItem => (old: shoppingListItems.First(i => i.Foodstuff.Equals(newItem.Foodstuff)), newer: newItem))
                .Map(items => shoppingListItems.Replace(items.old, items.newer))
                .Map(newItems => UpdateShoppingListItems(newItems));
        
        // Delete item
        
        private IOption<UserMessage> DeleteItem(ShoppingListItem item) =>
            ShoppingListHandler
                .RemoveFromShoppingList(environment, item, CurrentAccount)
                .Map(_ => UpdateShoppingListItems(shoppingListItems.Remove(item)))
                .MapToUserMessage(_ => Option.Empty<UserMessage>());
        
        // View model helpers

        private FoodstuffAmountCellViewModel ToViewModel(ShoppingListItem item) =>
            new FoodstuffAmountCellViewModel(
                item.Foodstuff,
                item.Amount,
                requiredAmounts.Get(item.Foodstuff),
                () => ShoppingListItemAction(item, i => ShoppingListHandler.Increase(i)),
                () => ShoppingListItemAction(item, i => ShoppingListHandler.Decrease(i)),
                new UserAction<Unit>(_ => DeleteItem(item).Async(), Icon.Delete(), order: 1)
            );

        private Unit UpdateShoppingListItems(IEnumerable<ShoppingListItem> newShoppingListItems) =>
            newShoppingListItems.OrderBy(i => i.Foodstuff.Name).ToImmutableList()
                .Pipe(newItems => shoppingListItems = newItems)
                .Pipe(_ => RaisePropertyChanged(nameof(ShoppingListItems)));
    }
}
