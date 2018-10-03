using System;
using System.Threading.Tasks;
using FuncSharp;

namespace SmartRecipes.Mobile.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<B> Map<A, B>(this Task<A> task, Func<A, B> mapper)
        {
            return mapper(await task);
        }
        
        public static async Task<B> Bind<A, B>(this Task<A> task, Func<A, Task<B>> binder)
        {
            return await binder(await task);
        }
        
        public static async Task<ITry<B, E>> BindTry<A, B, E>(this Task<ITry<A, E>> task, Func<A, Task<ITry<B, E>>> binder)
        {
            var a = await task;
            return await a.Match(
                s => binder(s),
                e => Try.Error<B, E>(e).ToCompletedTask()
            );
        }
        
        public static Task<Unit> ToUnit<A>(this Task<A> task)
        {
            return task.Map(_ => Unit.Value);
        }
        
        public static Task<Unit> ToUnit(this Task task)
        {
            return task.ContinueWith(_ => Unit.Value);
        }
    }

    public static class Tasks
    {
        public static Task<Unit> Unit()
        {
            return Task.FromResult(FuncSharp.Unit.Value);
        }
    }
}