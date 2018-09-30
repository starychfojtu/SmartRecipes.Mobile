namespace SmartRecipes.Mobile.Models
{
    public class AccessToken
    {
        public AccessToken(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}