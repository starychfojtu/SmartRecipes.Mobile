using System;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ViewModels
{
    public class FoodstuffAmountCellViewModel
    {
        public FoodstuffAmountCellViewModel(
            IFoodstuff foodstuff,
            IAmount amount,
            IOption<IAmount> requiredAmount,
            Func<Task> onPlus,
            Func<Task> onMinus,
            params UserAction<Unit>[] menuActions)
        {
            Foodstuff = foodstuff;
            Amount = amount;
            RequiredAmount = requiredAmount;
            OnPlus = onPlus;
            OnMinus = onMinus;
            MenuActions = menuActions;
        }

        public IFoodstuff Foodstuff { get; }

        public IAmount Amount { get; }

        public IOption<IAmount> RequiredAmount { get; }

        public Func<Task> OnPlus { get; }

        public Func<Task> OnMinus { get; }
        
        public UserAction<Unit>[] MenuActions { get; }
    }
}
