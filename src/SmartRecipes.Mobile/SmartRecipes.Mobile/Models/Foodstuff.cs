﻿using System;
using SQLite;

namespace SmartRecipes.Mobile.Models
{
    public class Foodstuff : Entity, IFoodstuff
    {
        private Foodstuff(Guid id, string name, Uri imageUrl, IAmount baseAmount, IAmount amountStep) : base(id)
        {
            Name = name;
            ImageUrl = imageUrl;
            BaseAmount = baseAmount;
            AmountStep = amountStep;
        }

        public Foodstuff() : base(Guid.Empty) { /* for sqlite */ }

        public string Name { get; set; }

        public string _ImageUrlString { get; set; }
        [Ignore]
        public Uri ImageUrl
        {
            get { return new Uri(_ImageUrlString); }
            set { _ImageUrlString = value.AbsoluteUri; }
        }

        public float _BaseCount { get; set; }
        public AmountUnit _BaseUnit { get; set; }
        [Ignore]
        public IAmount BaseAmount
        {
            get { return Amount.Create(_BaseCount, _BaseUnit); }
            set
            {
                _BaseCount = value.Count;
                _BaseUnit = value.Unit;
            }
        }

        public float _StepCount { get; set; }
        public AmountUnit _StepUnit { get; set; }
        [Ignore]
        public IAmount AmountStep
        {
            get { return Amount.Create(_StepCount, _StepUnit); }
            set
            {
                _StepCount = value.Count;
                _StepUnit = value.Unit;
            }
        }

        public AmountUnit Unit =>
            _BaseUnit;

        public static IFoodstuff Create(Guid id, string name, Uri imageUrl, IAmount baseAmount, IAmount amountStep)
        {
            return new Foodstuff(id, name, imageUrl, baseAmount, amountStep);
        }
    }
}
