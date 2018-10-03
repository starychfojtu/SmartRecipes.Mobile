using System;

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

        public bool IsValid { get; set; }
    }

    public static class ValidatableObject
    {
        public static ValidatableObject<T> Create<T>(Predicate<T> validate, Action<T> onDataChanged)
        {
            return new ValidatableObject<T>(default(T), validate, onDataChanged);
        }
    }
}
