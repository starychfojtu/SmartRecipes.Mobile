using System.Collections.Immutable;
using System.Net;
using Newtonsoft.Json;

namespace SmartRecipes.Mobile.Infrastructure
{
    public sealed class ApiError
    {
        public ApiError(string message, HttpStatusCode code)
        {
            var errors = JsonConvert.DeserializeObject<string[]>(message);
            Errors = errors.ToImmutableArray();
            Code = code;
        }

        public ImmutableArray<string> Errors { get; }
        
        public HttpStatusCode Code { get; }
    }
}