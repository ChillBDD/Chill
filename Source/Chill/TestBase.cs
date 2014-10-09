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
    /// Base class for all Chill tests. This baseclass set's up your automocking container. 
    /// 
    /// It also has a convenient method <see cref="TriggerTest"/> you can call that will trigger an async test func
    /// and capture any exceptions that might have occurred. 
    /// </summary>
    public abstract class TestBase: IDisposable
    {
        protected bool DefferedExecution;
        private bool testTriggered;

        protected Exception CaughtException
        {
            get
            {
                EnsureTestTriggered(expectExceptions:true);
                return caughtException;
            }
            set { caughtException = value; }
        }

    
        private IAutoMockingContainer container;
        private Exception caughtException;

        /// <summary>
        /// Automocking IOC container that you can use to build subjects. 
        /// </summary>
        public IAutoMockingContainer Container
        {
            get
            {
                EnsureContainer();
                return container;
            }
        }

        protected internal void EnsureTestTriggered(bool expectExceptions)
        {
            if (!testTriggered)
            {
                testTriggered = true;
                TriggerTest(expectExceptions);
            }
        }

        internal virtual void TriggerTest(bool expectExceptions)
        {
            
        }

        /// <summary>
        /// Trigger a test. If <see cref="ExpectExceptions"/> is true, it will catch any exception and set the
        /// <see cref="CaughtException"/> value to that. Otherwise, the action will just be executed. 
        /// </summary>
        /// <remarks>If <see cref="ExpectExceptions"/> is set and no exception is thrown, an exception will be thrown</remarks>
        /// <param name="testAction"></param>
        /// <param name="expectExceptions"></param>
        internal void TriggerTest(Func<Task> testAction, bool expectExceptions)
        {
            if (expectExceptions)
            {
                try
                {
                    testAction().Wait();
                }
                catch (AggregateException ex)
                {
                    CaughtException = ex.GetBaseException();
                }
                finally
                {
                    if (expectExceptions && CaughtException == null)
                        throw new InvalidOperationException("Expected exception but no exception was thrown");
                }
            }
            else
            {
                testAction().Wait();
            }
        }

        /// <summary>
        /// Makes sure the container has been created. 
        /// </summary>
        internal void EnsureContainer()
        {
            if (container == null)
            {
                container = BuildContainer();
            }
        }


        private static Type ContainerType;

        /// <summary>
        /// Creates an automocking container. By default, it will look in your assemblies to find an implementation 
        /// of <see cref="IAutoMockingContainer"/> and use that. You can override this method to customize how the container is being 
        /// created. 
        /// </summary>
        /// <returns>The automocking container that's used for this test. </returns>
        protected virtual IAutoMockingContainer BuildContainer()
        {
            if (ContainerType == null)
            {
                ContainerType = FindContainerType();
            }
            return (IAutoMockingContainer)Activator.CreateInstance(ContainerType);
        }

        /// <summary>
        /// Searches the loaded assemblies to find an implementation of <see cref="IAutoMockingContainer"/>
        /// </summary>
        /// <returns></returns>
        protected static Type FindContainerType()
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
            {
                throw new InvalidOperationException("More than one implementation of IContainer could be found: " +
                                                    String.Join(",", types.Select(x => x.Name)));
            }
            Type containerType = types.First();
            return containerType;
        }


        /// <summary>
        /// Cleans up all usages. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        public IStoreStateBuilder<T> SetThe<T>() where T : class
        {
            return new StoreStateBuilder<T>(this);
        }


        /// <summary>
        /// Get a value from the container. If you haven't explicitly registered any value, it will use the AutoMockingContainer to 
        /// create a mock object for you. However, you can use the <see cref="SetThe{T}"/> method to explicitly register an object or value. 
        /// </summary>
        /// <typeparam name="T">The type of object you wish to register</typeparam>
        /// <returns>An object of a specific type registered to the container. </returns>
        public T The<T>()
            where T : class
        {
            return Container.Get<T>();
        }

        /// <summary>
        /// Get a value from the container, identified by an index. This should have been set by using <see cref="StoreStateBuilderExtensions.AtIndex{T}"/>.
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

        /// <summary>
        /// Get a value from the container, identified by the name. This should have been set by using <see cref="StoreStateBuilderExtensions.Named{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="named"></param>
        /// <returns></returns>
        public T The<T>(string named)
            where T : class
        {
            var items = Container.Get<Dictionary<string, T>>();

            T item;
            if (!items.TryGetValue(named, out item))
            {
                throw new ArgumentException(string.Format("No object of type {0} was stored under name {1}", typeof(T).Name, named));
            }
            return item;
        }

        public IEnumerable<T> All<T>() where T : class
        {
            return Container.Get<List<T>>();
        }

        /// <summary>
        /// Short hand for setting <see cref="The{T}"/> to a value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueToSet"></param>
        /// <returns></returns>
        public T UseThe<T>(T valueToSet) where T : class
        {
            return Container.Set<T>(valueToSet);
        }

        /// <summary>
        /// Clean up all instances created by the container. 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Container.Dispose();
            }
        }
    }
}
