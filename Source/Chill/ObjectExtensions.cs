using System;
using System.Threading.Tasks;

namespace Chill
{
    public static class ObjectExtensions
    {
        public static TSubject With<TSubject, TReturnValue>(this TSubject subject, Func<TSubject, TReturnValue> func)
        {
            func(subject);
            return subject;
        }

        public static Task<T> Asynchronously<T>(this T subject)
        {
            return Task.Run(() => subject);
        }

    }
}