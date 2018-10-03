using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartRecipes.Mobile.ApiDto;

namespace SmartRecipes.Mobile.Oop
{   
    public class ApiClient
    {
        private readonly HttpClient client;
        
        public ApiClient(HttpClient client)
        {
            this.client = client;
        }

        public Task<SignInResponse> Post(SignInRequest request)
        {
            // Contract.RequiresEmail(request.email)
            // Contract.RequiresPassword(request.password)
            return Post<SignInRequest, SignInResponse>(request, "/signIn");
        }
        
        private async Task<TResponse> Post<TRequest, TResponse>(TRequest request, string route)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(route, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(responseContent, response.StatusCode);
            }
            
            return JsonConvert.DeserializeObject<TResponse>(responseContent);
        }
    }

    public class ApiException : Exception
    {
        public ApiException(string message, HttpStatusCode code) : base(message)
        {
            Code = code;
        }
        
        public HttpStatusCode Code { get; }
    }
}
