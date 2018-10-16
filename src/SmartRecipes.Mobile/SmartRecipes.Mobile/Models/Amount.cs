using System;
using FuncSharp;

namespace SmartRecipes.Mobile.Models
{
    public struct Amount : IAmount
    {
        private Amount(float count, AmountUnit unit)
        {
            Unit = unit;
            Count = count < 0 ? 0 : count;
        }

        public float Count { get; }

        public AmountUnit Unit { get; }

        public IAmount WithCount(float count)
        {
            return new Amount(count, Unit);
        }

        public override string ToString()
        {
            return $"{Count} {Unit.ToString()}";
        }

        public static IAmount Zero(AmountUnit unit)
        {
            return new Amount(0, unit);
        }

        public static bool IsLessThan(IAmount a1, IAmount a2)
        {
            return a1.Unit == a2.Unit && a1.Count < a2.Count;
        }

        public static IOption<IAmount> Add(IAmount a1, IAmount a2)
        {
            return CountOperation((c1, c2) => c1 + c2, a1, a2);
        }

        public static IOption<IAmount> Substract(IAmount a1, IAmount a2)
        {
            return CountOperation((c1, c2) => c1 - c2, a1, a2);
        }

        private static IOption<IAmount> CountOperation(Func<float, float, float> op, IAmount first, IAmount second)
        {
            var validOperation = first.Unit == second.Unit;
            var amount = validOperation
                ? new Amount(op(first.Count, second.Count), first.Unit).ToOption()
                : Option.Empty<Amount>();
            return amount.Map(a => a as IAmount);
        }
        
        public static IAmount Create(float count, AmountUnit unit)
        {
            return new Amount(count, unit);
        }
    }
}
