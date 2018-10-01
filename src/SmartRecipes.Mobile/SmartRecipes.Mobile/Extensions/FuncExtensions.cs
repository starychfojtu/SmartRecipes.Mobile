using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Infrastructure;

namespace SmartRecipes.Mobile.Extensions
{
    public static class FuncExtensions
    {
        public static string ToPropertyPathName<T, TProperty>(this Expression<Func<T, TProperty>> memberExpression)
        {
            var expressions = memberExpression.ToString();
            var indexOfFirstDot = expressions.IndexOf('.');
            var propertyPathName = expressions.Substring(indexOfFirstDot + 1);
            return propertyPathName;
        }

        public static Monad.Reader<E, Task<B>> Bind<E, A, B>(this Monad.Reader<E, Task<A>> reader, Func<A, Monad.Reader<E, Task<B>>> binder)
        {
            return reader.SelectMany(a => binder(a), (r1, r2) => r2);
        }

        public static Monad.Reader<E, Task<C>> SelectMany<E, A, B, C>(
            this Monad.Reader<E, Task<A>> reader,
            Func<A, Monad.Reader<E, Task<B>>> binder,
            Func<A, B, C> selector)
        {
            return env =>
            {
                var first = reader(env);
                var second = first.Bind(a => binder(a)(env));
                return first.Bind(a => second.Map(b => selector(a, b)));
            };
        }
        
        public static Monad.Reader<E, Task<B>> Select<E, A, B>(this Monad.Reader<E, Task<A>> reader, Func<A, B> selector)
        {
            return env => reader(env).Map(r => selector(r));
        }

        public static IOption<UserMessage> MapToUserMessage<A>(this ITry<A> aTry, Func<A, IOption<UserMessage>> mapper)
        {
            return aTry.Match(r => mapper(r), e => Option.Create(UserMessage.Error(e)));
        }
    }
}
