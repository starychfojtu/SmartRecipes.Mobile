using FuncSharp;

namespace SmartRecipes.Mobile.Infrastructure
{
    public class UserActionResult
    {
        private UserActionResult(bool isSuccess, IOption<UserMessage> message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; }
        
        public IOption<UserMessage> Message { get; }

        public static UserActionResult Success(UserMessage message = null)
        {
            return new UserActionResult(isSuccess: true, message: message.ToOption());
        }
        
        public static UserActionResult Error(UserMessage message = null)
        {
            return new UserActionResult(isSuccess: false, message: message.ToOption());
        }
    }
}