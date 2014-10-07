using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Chill.StateBuilders;

namespace Chill
{
    /// <summary>
    /// Base class for tests that use a similar appraoch. 
    /// </summary>
    public abstract class TestBase: IDisposable
    {
        protected bool ExpectExceptions;
        protected Exception CaughtException;
        private IAutoMockingContainer container;

        public IAutoMockingContainer Container
        {
            get
            {
                EnsureContainer();
                return container;
            }
        }

        internal void TriggerTest(Func<Task> testAction)
        {
            if (ExpectExceptions)
            {
                try
                {
                    testAction().Wait();
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
            else
            {
                testAction().Wait();
            }
        }

        internal void EnsureContainer()
        {
            if (container == null)
            {
                container = BuildContainer();
            }
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


        public void Dispose()
        {
            Dispose(true);
        }

        public IStoreStateBuilder<T> SetThe<T>() where T : class
        {
            return new StoreStateBuilder<T>(this);
        }


        /// <summary>
        /// Create a substitute and use that from now on. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T The<T>()
            where T : class
        {
            return Container.Get<T>();
        }

        /// <summary>
        /// Create a substitute and use that from now on. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T The<T>(int atIndex)
            where T : class
        {
            var items = Container.Get<List<T>>();

            if (atIndex < items.Count)
            {
                return items[atIndex];
            }

            throw new ArgumentException(string.Format("No object of type {0} was stored at index {1}", typeof(T).Name, atIndex));
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
