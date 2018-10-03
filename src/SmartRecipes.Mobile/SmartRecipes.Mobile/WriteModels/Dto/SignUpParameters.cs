using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.WriteModels.Dto
{
    public class SignUpParameters
    {
        public SignUpParameters(Credentials credentials)
        {
            Credentials = credentials;
        }

        public Credentials Credentials { get; }
    }
}