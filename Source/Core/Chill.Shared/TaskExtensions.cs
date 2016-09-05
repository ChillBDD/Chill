using System;
using System.Threading.Tasks;

namespace Chill
{
    internal static class TaskExtensions
    {
        public static void WaitAndFlattenExceptions(this Task t)
        {
            try
            {
#if NET45
                Task.Run(() => t.Wait()).Wait();
#else
                Task.Factory.StartNew(t.Wait).Wait();
#endif
            }
            catch (AggregateException aggregateException)
            {
                throw aggregateException.Flatten();
            }
        }
    }
}