using System;
using FuncSharp;

namespace SmartRecipes.Mobile.Infrastructure
{
    public sealed class ApiResult<T> : Coproduct3<T, ApiError, NoConnection>
    {
        public ApiResult(T response) : base(response)
        {
        }

        public ApiResult(ApiError error) : base(error)
        {
        }

        public ApiResult(NoConnection thirdValue) : base(thirdValue)
        {
        }

        public TResult ContractMatchSuccess<TResult>(Func<T, TResult> ifSuccess, Func<NoConnection, TResult> ifNoConnection)
        {
            return Match(
                ifSuccess,
                _ => throw new InvalidOperationException("Internal error."),
                ifNoConnection
            );
        }
    }
    
    public static class ApiResult
    {
        public static ApiResult<T> Success<T>(T value)
        {
            return new ApiResult<T>(value);
        }
        
        public static ApiResult<T> Error<T>(ApiError value)
        {
            return new ApiResult<T>(value);
        }
        
        public static ApiResult<T> NoConnection<T>()
        {
            return new ApiResult<T>(new NoConnection());
        }
    }

    public sealed class NoConnection
    {
    }
}