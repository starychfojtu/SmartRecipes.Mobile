using System.Collections.Generic;

namespace SmartRecipes.Mobile.Infrastructure
{
    public sealed class ApiError
    {
        public ApiError(string message, IEnumerable<ApiParameterError> parameterErrors)
        {
            Message = message;
            ParameterErrors = parameterErrors;
        }
        
        public string Message { get; }
        
        public IEnumerable<ApiParameterError> ParameterErrors { get; }
    }
    
    public sealed class ApiParameterError
    {
        public ApiParameterError(string message, string field)
        {
            Message = message;
            Field = field;
        }

        public string Message { get; }
        
        public string Field { get; }
    }
}