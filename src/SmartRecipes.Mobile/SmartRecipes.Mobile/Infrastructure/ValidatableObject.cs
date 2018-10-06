using System;
using FuncSharp;

namespace SmartRecipes.Mobile.Infrastructure
{
    public sealed class ValidatableObject<T>
    {
        private T value;

        private readonly Action<T> onDataChanged;

        private readonly Predicate<T> validate;

        public ValidatableObject(T value, Predicate<T> validate, Action<T> onDataChanged)
        {
            this.value = value;
            this.validate = validate;
            this.onDataChanged = onDataChanged;
            IsValid = validate(value);
        }

        public T Value
        {
            get { return value; }
            set
            {
                this.value = value;
                IsValid = validate(value);
                onDataChanged?.Invoke(value);
            }
        }

        public bool IsValid { get; private set; }

        public Unit Invalidate()
        {
            IsValid = false;
            onDataChanged?.Invoke(Value);
            return Unit.Value;
        }
    }

    public static class ValidatableObject
    {
        public static ValidatableObject<T> Create<T>(Predicate<T> validate, Action<T> onDataChanged)
        {
            return new ValidatableObject<T>(default(T), validate, onDataChanged);
        }
    }
}
