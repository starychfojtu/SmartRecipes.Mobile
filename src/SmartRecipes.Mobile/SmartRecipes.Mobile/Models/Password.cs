using System;
using FuncSharp;

namespace SmartRecipes.Mobile.Models
{
    public sealed class Password
    {
        private Password(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static ITry<Password> Create(string value)
        {
            if (value is null)
            {
                return Try.Error<Password>(new Exception("Password cannot be empy."));
            }

            var minPasswordLength = 6;
            if (value.Length < minPasswordLength)
            {
                return Try.Error<Password>(new Exception($"Password must be at least {minPasswordLength} characters long."));
            }

            return Try.Success(new Password(value));
        }
    }
}