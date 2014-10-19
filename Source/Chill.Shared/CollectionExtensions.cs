using System;
using System.Collections.Generic;

namespace Chill
{
    public static class CollectionExtensions
    {
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