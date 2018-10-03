using System;

namespace SmartRecipes.Mobile.Infrastructure
{
    public static class Contract
    {
        public static void Failed(string message = null)
        {
            throw new InvalidOperationException(message);
        }
    }
}