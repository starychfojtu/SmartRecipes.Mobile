using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Monad;
using SmartRecipes.Mobile.Controls;
using SmartRecipes.Mobile.Infrastructure;
using Xamarin.Forms;
using Environment = SmartRecipes.Mobile.Infrastructure.Environment;

namespace SmartRecipes.Mobile.Extensions
{
    public static class ObjectExtensions
    {
        public static Task<T> Async<T>(this T obj) =>
            Task.FromResult(obj);

        public static Reader<Environment, T> ToReader<T>(this T obj) =>
            env => obj;
        
        public static B Pipe<A, B>(this A obj, Func<A, B> f) => 
            f(obj);
        
        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            yield return obj;
        }

        public static T Tee<T>(this T obj, Action<T> action)
        {
            action(obj);
            return obj;
        }

        public static async Task<T> TeeAsync<T>(this T obj, Func<T, Task> action)
        {
            await action(obj);
            return obj;
        }

        public static void BindValue<TContext, TProperty>(this TContext context, BindableObject obj, BindableProperty property, Expression<Func<TContext, TProperty>> propertyAccessor)
        {
            obj.SetBinding(property, propertyAccessor.ToPropertyPathName());
        }

        public static void BindText<TContext, TProperty>(this TContext context, Entry entry, Expression<Func<TContext, TProperty>> propertyAccessor)
        {
            context.BindValue(entry, Entry.TextProperty, propertyAccessor);
        }

        public static void BindErrors<TContext>(this TContext context, ValidatableEntry entry, Expression<Func<TContext, bool>> predicate)
        {
            context.BindValue(entry, ValidatableEntry.IsValidProperty, predicate);
        }
    }
}
