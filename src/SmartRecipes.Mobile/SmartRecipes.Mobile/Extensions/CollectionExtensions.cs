﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SmartRecipes.Mobile.Infrastructure;
using Environment = System.Environment;

namespace SmartRecipes.Mobile.Extensions
{
    public static class CollectionExtensions
    {
        public static IImmutableList<T> ReplaceItem<T>(this IImmutableList<T> list, T item, T replacement)
        {
            return list.Remove(item).Add(replacement);
        }

        public static ImmutableDictionary<A, B> Merge<A, B>(
            this ImmutableDictionary<A, B> first,
            ImmutableDictionary<A, B> second,
            Func<B, B, B> resolve)
        {
            return second.Aggregate(first, (fst, kvp) => fst.SetItem(
                kvp.Key, 
                fst.ContainsKey(kvp.Key) ? resolve(fst[kvp.Key], kvp.Value) : kvp.Value
            ));
        }

        public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (list is List<T>)
            {
                ((List<T>)list).AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
            return list;
        }
        
        public static void Deconstruct<A, B>(this KeyValuePair<A, B> kvp, out A key, out B value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }

        public static UserActionResult ToUserActionResult(this IEnumerable<Exception> exceptions)
        {
            var message = exceptions.Select(e => e.Message).Aggregate((s1, s2) => $"{s1}{Environment.NewLine}{s2}");
            return UserActionResult.Error(new UserMessage("Error", message));
        }
    }
}
