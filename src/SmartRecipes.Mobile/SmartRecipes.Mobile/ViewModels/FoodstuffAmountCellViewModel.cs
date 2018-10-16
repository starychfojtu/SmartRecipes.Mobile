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
            float amount,
            IOption<float> requiredAmount,
            Func<Unit, Task> onPlus,
            Func<Unit, Task> onMinus,
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

        public float Amount { get; }

        public IOption<float> RequiredAmount { get; }

        public Func<Unit, Task> OnPlus { get; }

        public Func<Unit, Task> OnMinus { get; }
        
        public UserAction<Unit>[] MenuActions { get; }
    }
}
