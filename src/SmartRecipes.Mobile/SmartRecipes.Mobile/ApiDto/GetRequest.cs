using System.Collections.Specialized;

namespace SmartRecipes.Mobile.ApiDto
{
    public interface IGetRequest
    {
        NameValueCollection ToQueryString();
    }
}