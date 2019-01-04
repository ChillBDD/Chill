using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Chill.StateBuilders;

namespace Chill
{
    /// <summary>
    /// Base class for all Chill tests. This baseclass set's up your automocking container. 
    /// 
    /// It also has a convenient method TriggerTest you can call that will trigger an async test func
    /// and capture any exceptions that might have occurred. 
    /// </summary>
    public abstract partial class TestBase : IDisposable
    {
        /// <summary>
        /// Should the test execution start immediately on the When method or should execution be deferred until needed. 
        /// </summary>
        [Obsolete("Because of the typo and will be removed. Consider using DeferredExecution property instead.")]
        protected bool DefferedExecution
        {
            get { return DeferredExecution; }
            set { DeferredExecution = value; }
        }

        /// <summary>
        /// Should the test execution start immediately on the When method or should execution be deferred until needed. 
        /// </summary>
        protected bool DeferredExecution { get; set; }

        private bool testTriggered;
        private bool containerInitialized;
        internal readonly IChillContainerInitializer ChillContainerInitializer;

        /// <summary>
        /// Creates a new instance of the testbase and creates the TestInitializer from the attribute
        /// </summary>
        public TestBase()
        {
            ChillContainerInitializer = BuildInitializer();
        }

        /// <summary>
        /// Any exception that might be thrown in the course of executing the When Method. Note, this property is often used
        /// in conjunction with deferred excecution. 
        /// </summary>
        protected Exception CaughtException
        {
            get
            {
                EnsureTestTriggered(expectExceptions: true);
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

        /// <summary>
        /// Method that ensures that the test has actually been triggered. 
        /// </summary>
        /// <param name="expectExceptions"></param>
        protected internal void EnsureTestTriggered(bool expectExceptions)
        {
            if (!testTriggered)
            {
                testTriggered = true;
                TriggerTest(expectExceptions);
            }
        }

        /// <summary>
        /// Method that can be overriden to trigger the actual test
        /// </summary>
        /// <param name="expectExceptions"></param>
        internal virtual void TriggerTest(bool expectExceptions)
        {
        }

        internal void TriggerTest(Action testAction, bool expectExceptions)
        {
            if (expectExceptions)
            {
                try
                {
                    testAction();
                }
                catch (AggregateException ex)
                {
                    CaughtException = ex.GetBaseException();
                }
                catch (Exception ex)
                {
                    CaughtException = ex;
                }
                finally
                {
                    if (expectExceptions && CaughtException == null)
                        throw new InvalidOperationException("Expected exception but no exception was thrown");
                }
            }
            else
            {
                testAction();
            }
        }

        /// <summary>
        /// Makes sure the container has been created. 
        /// </summary>
        protected virtual void EnsureContainer()
        {
            if (container == null)
            {
                container = ChillContainerInitializer.BuildChillContainer(this);
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

        /// <summary>
        /// Initialize the Chill container
        /// </summary>
        protected virtual void InitializeContainer()
        {
            ChillContainerInitializer.InitializeContainer(this);
        }

        /// <summary>
        /// Creates an automocking container. By default, it will look in your assemblies to find an implementation 
        /// of <see cref="IChillContainer"/> and use that. You can override this method to customize how the container is being 
        /// created. 
        /// </summary>
        /// <returns>The automocking container that's used for this test. </returns>
        public virtual IChillContainer BuildContainer(Type containerType) 
        {
            return (IChillContainer)Activator.CreateInstance(containerType);
        }

        /// <summary>
        /// Searches the loaded assemblies to find an implementation of <see cref="IChillContainer"/>
        /// </summary>
        /// <returns></returns>
        protected IChillContainerInitializer BuildInitializer()
        {
            object attribute =
                GetType()
                    .GetTypeInfo()
                    .GetCustomAttributes(typeof (ChillContainerInitializerAttribute), false)
                    .SingleOrDefault() ??
                GetType()
                    .GetTypeInfo()
                    .Assembly.GetCustomAttributes(typeof (ChillContainerInitializerAttribute))
                    .SingleOrDefault();

            GetBuiltInContainer(ref attribute);

            if (attribute == null)
            {

                throw new InvalidOperationException(
                    "Could not find the Chill Container. You must have a Chill container registered using the ChillContainerInitializer. Get the Chill Container from one of the extensions. ");
            }
            var type = ((ChillContainerInitializerAttribute) attribute).ChillContainerInitializerType;

            if (type == null)
            {
                throw new InvalidOperationException(
                    "The type property on the ChillContainerInitializerAttribute should not be null");
            }

            return (IChillContainerInitializer) Activator.CreateInstance(type);
        }

        partial void GetBuiltInContainer(ref object attribute) ;

        /// <summary>
        /// Cleans up all usages. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Fluent Syntax for storing a value in the Chill Container. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
            T value = Container.Get<T>();

            if (!Container.IsRegistered<T>())
            {
                Container.Set(value);
            }

            return value;
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
            var items = Container.Get<Dictionary<Tuple<Type, string>, object>>();
            var key = Tuple.Create(typeof (T), named);

            object item;
            if (!items.TryGetValue(key, out item))
            {
                item = Container.Get<T>(named);
                items.Add(key, item);
                container.Set(items);
            }
            return (T) item;
        }

        /// <summary>
        /// Allows you to store multiple items of a given type in the chill container. Often used in conjunction with the <see cref="All{T}"/> method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public IEnumerable<T> SetAll<T>(params T[] items)
            where T : class
        {
            return container.AddToList(items);
        }

        /// <summary>
        /// Allows you to store multiple items of a given type in the chill container. Often used in conjunction with the <see cref="All{T}"/> method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public IEnumerable<T> SetAll<T>(IEnumerable<T> items)
            where T : class
        {
            return container.AddToList(items.ToArray());
        }

        /// <summary>
        /// Returns all items registered in the chill container to a specific interface. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
        /// Short hand for setting <see cref="The{T}"/> to a value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueToSet"></param>
        /// <param name="named"></param>
        /// <returns></returns>
        public T UseThe<T>(T valueToSet, string named) where T : class
        {
            return Container.Set<T>(valueToSet, named);
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