using System;
using System.Collections.Generic;

namespace Chill
{
    /// <summary>
    /// Extensions to the Chill container
    /// </summary>
    internal static class AutoMockingContainerExtensions
    {
        /// <summary>
        /// chill keeps a list of registered items in memory. This method adds a value to that list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="itemsToAdd"></param>
        /// <returns></returns>
        internal static IEnumerable<T> AddToList<T>(this IChillContainer container, params T[] itemsToAdd)
            where T : class
        {
            var list = GetList<T>(container);

            list.AddRange(itemsToAdd);


            return list;
        }

        /// <summary>
        /// chill keeps a list of registered items in memory. This method get's that list. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        internal static List<T> GetList<T>(this IChillContainer container) where T : class
        {
            var dictionary = container.Get<Dictionary<Type, object>>();


            if (dictionary == null || dictionary.Count == 0)
            {
                dictionary = new Dictionary<Type, object>();
            }

            object list;
            if (!dictionary.TryGetValue(typeof (T), out list))
            {
                list = new List<T>();
                dictionary.Add(typeof (T), list);
                container.Set(dictionary);
            }
            return (List<T>) list;
        }
    }
}