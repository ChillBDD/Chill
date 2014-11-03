using System;
using System.Collections.Generic;

namespace Chill
{
    /// <summary>
    /// Extensions to collections 
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds x number of items to a collection using the specified factory method. 
        /// </summary>
        /// <typeparam name="TSubject"></typeparam>
        /// <param name="subject"></param>
        /// <param name="count"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static ICollection<TSubject> Add<TSubject>(this ICollection<TSubject> subject, int count, Func<int, TSubject> factory)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            for (int i = 0; i < count; i++)
            {
                subject.Add(factory(i));
            }

            return subject;
        }
    }
}