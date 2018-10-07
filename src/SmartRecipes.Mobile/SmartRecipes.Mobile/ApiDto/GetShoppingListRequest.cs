using System.Collections.Specialized;

namespace SmartRecipes.Mobile.ApiDto
{
    public class GetShoppingListRequest : IGetRequest
    {
        public NameValueCollection ToQueryString() =>
            new NameValueCollection();
    }
}