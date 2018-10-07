namespace SmartRecipes.Mobile.Models
{
    public interface IAmount
    {
        float Count { get; }

        AmountUnit Unit { get; }

        IAmount WithCount(float count);
    }
}
