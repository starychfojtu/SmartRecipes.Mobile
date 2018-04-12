﻿using System;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile
{
    public class FoodstuffCellViewModel
    {
        public FoodstuffCellViewModel(Foodstuff foodstuff, Amount amount, Func<Task> onPlus, Func<Task> onMinus = null)
        {
            Foodstuff = foodstuff;
            Amount = amount;
            OnPlus = onPlus;
            OnMinus = onMinus;
        }

        public Foodstuff Foodstuff { get; }

        public Amount Amount { get; }

        public Func<Task> OnPlus { get; }

        public Func<Task> OnMinus { get; }
    }
}
