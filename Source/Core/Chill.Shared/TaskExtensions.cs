using System;
using System.Threading.Tasks;

namespace Chill
{
    internal static class TaskExtensions
    {
        public static void WaitAndFlattenExceptions(this Task t)
        {
#if NET45
            Task.Run(() => t.GetAwaiter().GetResult()).GetAwaiter().GetResult();
#else
            Task.Factory.StartNew(() => t.GetAwaiter().GetResult()).GetAwaiter().GetResult();
#endif
        }

        public static TResult WaitAndFlattenExceptions<TResult>(this Task<TResult> t)
        {
#if NET45
            return Task.Run(() => t.GetAwaiter().GetResult()).GetAwaiter().GetResult();
#else
            return Task.Factory.StartNew(() => t.GetAwaiter().GetResult()).GetAwaiter().GetResult();
#endif
        }
    }
}