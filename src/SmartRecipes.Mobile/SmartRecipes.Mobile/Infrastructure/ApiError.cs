using System.Net;

namespace SmartRecipes.Mobile.Infrastructure
{
    public sealed class ApiError
    {
        public ApiError(string message, HttpStatusCode code)
        {
            Message = message;
            Code = code;
        }

        public string Message { get; }
        
        public HttpStatusCode Code { get; }
    }
}