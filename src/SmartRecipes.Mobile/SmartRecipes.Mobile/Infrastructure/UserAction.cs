using System;
using System.Threading.Tasks;
using FuncSharp;

namespace SmartRecipes.Mobile.Infrastructure
{
    public sealed class UserAction<T>
    {
        public UserAction(Func<T, Task<IOption<UserMessage>>> action, Icon icon, int order)
        {
            Action = action;
            Icon = icon;
            Order = order;
        }

        public Func<T, Task<IOption<UserMessage>>> Action { get; }
        
        public Icon Icon { get; }
        
        public int Order { get; }
    }
}