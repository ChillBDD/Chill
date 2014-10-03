using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestStack
{
    /// <summary>
    /// Base class for tests that use a similar appraoch. 
    /// </summary>
    public abstract class TestBase: IDisposable
    {
        protected bool ExpectExceptions;
        protected Exception CaughtException;
        protected IAutoMockingContainer Container;

        protected virtual void OnWhen()
        {
            try
            {
                TriggerWhen();
            }
            catch (Exception ex)
            {
                CaughtException = ex;
            }
            finally
            {
                if (ExpectExceptions && CaughtException == null)
                    throw new InvalidOperationException("Expected exception but no exception was thrown");
            }
        }

        protected virtual async Task TriggerWhen()
        {
            
        }



        protected virtual void EnsureContainer()
        {
            if (Container == null)
                Container = BuildContainer();
        }

        public void Given(Action a)
        {
            EnsureContainer();
            a();
        }


        private static Type ContainerType;

        protected virtual IAutoMockingContainer BuildContainer()
        {
            if (ContainerType == null)
            {
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(IAutoMockingContainer).IsAssignableFrom(p) && p.IsClass)
                    .ToList();

                if (!types.Any())
                {
                    throw new InvalidOperationException("No implementation of IContainer could be found.");
                }

                if (types.Count() > 1)
                    throw new InvalidOperationException("More than one implementation of IContainer could be found: " +
                                                        String.Join(",", types.Select(x => x.Name)));
                ContainerType = types.First();
            }
            return (IAutoMockingContainer)Activator.CreateInstance(ContainerType);
        }


        /// <summary>
        /// Create a substitute and use that from now on. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T The<T>()
            where T : class
        {
            return Container.Get<T>();
        }

        /// <summary>
        /// Configure the container to use a specific subject from now on. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected T Store<T>(T subject, string key = null)
            where T : class
        {
            Container.Set<T>(subject);
            return subject;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Container.Dispose();
            }
        }
    }
}
