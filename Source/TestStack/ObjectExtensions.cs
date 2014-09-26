using System;

namespace TestStack
{
    public static class ObjectExtensions
    {
        public static TSubject With<TSubject, TReturnValue>(this TSubject subject, Func<TSubject, TReturnValue> func)
        {
            func(subject);
            return subject;
        }

    }
}