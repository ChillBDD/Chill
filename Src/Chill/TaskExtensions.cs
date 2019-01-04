using System;

#if NET45
using System.Threading;
#endif

using System.Threading.Tasks;

namespace Chill
{
    internal static class TaskExtensions
    {
        /// <summary>
        /// Some unit test frameworks (like xUnit) have their own synchronization context
        /// that does not work well with blocking waits and can lead to deadlocks.
        /// This method creates the task in the default synchronization context
        /// and blocks until the task is completed.
        /// </summary>
        /// <param name="taskFactory">The factory delegate that creates the task.</param>
        public static void ExecuteInDefaultSynchronizationContext(this Func<Task> taskFactory)
        {
#if NET45
            SynchronizationContext originalSynchronizationContext = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                taskFactory().GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(originalSynchronizationContext);
            }
#else
            Task.Factory.StartNew(() => taskFactory().GetAwaiter().GetResult()).GetAwaiter().GetResult();
#endif
        }

        /// <summary>
        /// Some unit test frameworks (like xUnit) have their own synchronization context
        /// that does not work well with blocking waits and can lead to deadlocks.
        /// This method creates the task in the default synchronization context
        /// and blocks until the task is completed.
        /// </summary>
        /// <param name="taskFactory">The factory delegate that creates the task.</param>
        public static TResult ExecuteInDefaultSynchronizationContext<TResult>(this Func<Task<TResult>> taskFactory)
        {
#if NET45
            SynchronizationContext originalSynchronizationContext = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                return taskFactory().GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(originalSynchronizationContext);
            }
#else
            return Task.Factory.StartNew(() => taskFactory().GetAwaiter().GetResult()).GetAwaiter().GetResult();
#endif
        }
    }
}