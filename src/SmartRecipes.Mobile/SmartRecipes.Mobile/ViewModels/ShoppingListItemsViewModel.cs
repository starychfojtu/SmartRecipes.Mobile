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
        private readonly Environment _environment;

        private IImmutableList<ShoppingListItem> shoppingListItems { get; set; }

        private IImmutableDictionary<IFoodstuff, IAmount> requiredAmounts { get; set; }

        public ShoppingListItemsViewModel(Environment environment)
        {
            this._environment = environment;
            shoppingListItems = ImmutableList.Create<ShoppingListItem>();
        }

        public IEnumerable<FoodstuffAmountCellViewModel> ShoppingListItems
        {
            get { return shoppingListItems.Select(i => ToViewModel(i)); }
        }

        public override async Task InitializeAsync()
        {
            requiredAmounts = await ShoppingListRepository.GetRequiredAmounts(CurrentAccount)(_environment);
            UpdateShoppingListItems(await ShoppingListRepository.GetItems(CurrentAccount.Id)(_environment));
        }

        public async Task Refresh()
        {
            await InitializeAsync();
        }

        public async Task OpenAddFoodstuffDialog()
        {
            var selected = await Navigation.SelectFoodstuffDialog();
            var newShoppingListItems = await ShoppingListHandler.AddToShoppingList(_environment, CurrentAccount, selected);
            var allShoppingListItems = shoppingListItems.Concat(newShoppingListItems);
            UpdateShoppingListItems(allShoppingListItems);
        }

        private async Task ShoppingListItemAction(ShoppingListItem shoppingListItem, Func<ShoppingListItem, IShoppingListItemAmount> action)
        {
            var newAmount = action(shoppingListItem);
            var newShoppingListItem = shoppingListItem.WithItemAmount(newAmount);

            var oldItem = shoppingListItems.First(i => i.Foodstuff.Equals(shoppingListItem.Foodstuff));
            var newShoppingListItems = CollectionExtensions.Replace(shoppingListItems, oldItem, newShoppingListItem);

            await ShoppingListHandler.Update(_environment, newShoppingListItem.ItemAmount.ToEnumerable().ToImmutableList());
            UpdateShoppingListItems(newShoppingListItems);
        }
        
        private IOption<UserMessage> DeleteItem(ShoppingListItem item)
        {
            return ShoppingListHandler.RemoveFromShoppingList(_environment, item, CurrentAccount).MapToUserMessage(_ =>
            {
                UpdateShoppingListItems(shoppingListItems.Remove(item));
                return Option.Empty<UserMessage>();
            });
        }

        private Unit UpdateShoppingListItems(IEnumerable<ShoppingListItem> newShoppingListItems)
        {
            shoppingListItems = newShoppingListItems.OrderBy(i => i.Foodstuff.Name).ToImmutableList();
            RaisePropertyChanged(nameof(ShoppingListItems));
            return Unit.Value;
        }

        private FoodstuffAmountCellViewModel ToViewModel(ShoppingListItem item)
        {
            return new FoodstuffAmountCellViewModel(
                item.Foodstuff,
                item.Amount,
                requiredAmounts.Get(item.Foodstuff),
                () => ShoppingListItemAction(item, i => ShoppingListHandler.Increase(i)),
                () => ShoppingListItemAction(item, i => ShoppingListHandler.Decrease(i)),
                new UserAction<Unit>(_ => Task.FromResult(DeleteItem(item)), Icon.Delete(), 1)
            );
        }
    }
}
