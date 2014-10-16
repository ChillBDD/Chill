using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        private bool containerInitialized;
        internal readonly IChillTestInitializer ChillTestInitializer;
        public TestBase()
        {
            ChillTestInitializer = BuildInitializer();
        }

        protected Exception CaughtException
        {
            get
            {
                EnsureTestTriggered(expectExceptions:true);
                return caughtException;
            }
            set { caughtException = value; }
        }

    
        private IChillContainer container;
        private Exception caughtException;

        /// <summary>
        /// Automocking IOC container that you can use to build subjects. 
        /// </summary>
        public IChillContainer Container
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
            EnsureContainerInitialized();
        }

        private void EnsureContainerInitialized()
        {
            if (!containerInitialized)
            {
                containerInitialized = true;
                InitializeContainer();
            }
        }

        protected virtual void InitializeContainer()
        {
            ChillTestInitializer.InitializeContainer(this);
        }

        /// <summary>
        /// Creates an automocking container. By default, it will look in your assemblies to find an implementation 
        /// of <see cref="IChillContainer"/> and use that. You can override this method to customize how the container is being 
        /// created. 
        /// </summary>
        /// <returns>The automocking container that's used for this test. </returns>
        protected virtual IChillContainer BuildContainer()
        {
            return ChillTestInitializer.BuildChillContainer();
        }

        /// <summary>
        /// Searches the loaded assemblies to find an implementation of <see cref="IChillContainer"/>
        /// </summary>
        /// <returns></returns>
        protected IChillTestInitializer BuildInitializer()
        {
            var attribute = this.GetType().GetCustomAttributes(typeof (ChillTestInitializerAttribute)).SingleOrDefault() ??
                            this.GetType().Assembly.GetCustomAttributes(typeof(ChillTestInitializerAttribute)).SingleOrDefault();

            if (attribute == null)
            {
                throw new InvalidOperationException("Could not find the Chill Container. You must have a Chill container registered using the ChillTestInitializer. Get the Chill Container from one of the extensions. ");
            }
            var type = ((ChillTestInitializerAttribute)attribute).ChillTestContextType;

            if (type == null)
            {
                throw new InvalidOperationException("The type property on the ChillTestInitializerAttribute should not be null");
            }

            return (IChillTestInitializer)Activator.CreateInstance(type);
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
        /// Get a value from the container, identified by the name. This should have been set by using <see cref="StoreStateBuilderExtensions.Named{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="named"></param>
        /// <returns></returns>
        public T TheNamed<T>(string named)
            where T : class
        {
            var items = Container.Get<Dictionary<string, object>>(typeof(T).AssemblyQualifiedName);

            object item;
            if (!items.TryGetValue(named, out item))
            {
                item = Container.Get<T>(named);
                items.Add(named,item);
                container.Set(items, typeof(T).AssemblyQualifiedName);
            }
            return (T)item;
        }

        public IEnumerable<T> SetAll<T>(params T[] items)
            where T : class
        {
            return container.AddToList(items);
        }

        public IEnumerable<T> SetAll<T>(IEnumerable<T> items )
            where T : class
        {
            return container.AddToList(items.ToArray());
        } 

        public IEnumerable<T> All<T>() where T : class
        {
            return Container.GetList<T>();
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

    public static class AutoMockingContainerExtensions
    {

        internal static IEnumerable<T> AddToList<T>(this IChillContainer container, params T[] itemsToAdd)
            where T:class
        {
            var list = GetList<T>(container);

            list.AddRange(itemsToAdd);


            return list;
        }

        internal static List<T> GetList<T>(this IChillContainer container) where T : class
        {
            var dictionary = container.Get<Dictionary<Type, object>>();


            if (dictionary == null || dictionary.Count == 0)
            {
                dictionary = new Dictionary<Type, object>();
            }

            object list;
            if (!dictionary.TryGetValue(typeof (T), out list))
            {
                list = new List<T>();
                dictionary.Add(typeof (T), list);
                container.Set(dictionary);
            }
            return (List<T>)list ;
        }
    }
}
