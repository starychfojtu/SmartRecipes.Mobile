using System.Collections.Immutable;
using FuncSharp;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class RequiredAmounts
    {
        private readonly IImmutableDictionary<IFoodstuff, float> requirements;

        public RequiredAmounts(IImmutableDictionary<IFoodstuff, float> requirements)
        {
            this.requirements = requirements;
        }

        public IOption<float> Get(IFoodstuff foodstuff) =>
            requirements.Get(foodstuff);
    }
}