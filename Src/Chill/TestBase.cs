using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chill.Common;
using Chill.StateBuilders;

namespace Chill
{
    /// <summary>
    /// Base class for all Chill tests. This base class sets up your container and object mothers. 
    /// 
    /// It also has a convenient method <see cref="TriggerTest(System.Action,bool)"/> you can call that will trigger an async test func
    /// and capture any exceptions that might have occurred. 
    /// </summary>
    /// <remarks>
    /// We purposely named this class so badly because we want to avoid a consumer of our package to
    /// accidentally select it when deriving from a class starting with 'G'.
    /// </remarks>
    public abstract class TestBase : IDisposable
    {
        private bool testTriggered;

        private bool containerInitialized;

        private ObjectMotherContainerDecorator decorator;

        private Exception caughtException;

        private readonly IChillContainerInitializer chillContainerInitializer;

        /// <summary>
        /// Creates a new instance of the class and creates the TestInitializer from the attribute
        /// </summary>
        protected TestBase()
        {
            chillContainerInitializer = BuildInitializer();
        }

        /// <summary>
        /// Should the test execution start immediately on the When method or should execution be deferred until needed. 
        /// </summary>
        protected bool DeferredExecution { get; set; }

        /// <summary>
        /// Any exception that might be thrown in the course of executing the When Method. Note, this property is often used
        /// in conjunction with deferred execution. 
        /// </summary>
        protected Exception CaughtException
        {
            get
            {
                EnsureTestTriggered(expectExceptions: true);
                return caughtException;
            }
            private set { caughtException = value; }
        }

        /// <summary>
        /// The IOC container that you can use to build subjects. 
        /// </summary>
        public IChillContainer Decorator
        {
            get
            {
                EnsureContainer();
                return decorator;
            }
        }

        /// <summary>
        /// Method that ensures that the test has actually been triggered. 
        /// </summary>
        /// <param name="expectExceptions"></param>
        protected void EnsureTestTriggered(bool expectExceptions)
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

        protected void TriggerTest(Action testAction, bool expectExceptions)
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
                    if (CaughtException == null)
                    {
                        throw new InvalidOperationException("Expected exception but no exception was thrown");
                    }
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
            if (decorator == null)
            {
                decorator = new ObjectMotherContainerDecorator(chillContainerInitializer.BuildChillContainer(this));
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
            chillContainerInitializer.InitializeContainer(this);
            decorator.LoadAutoMothers(new[] {GetType().GetTypeInfo().Assembly});
        }

        /// <summary>
        /// Searches the loaded assemblies to find an implementation of <see cref="IChillContainer"/>
        /// </summary>
        /// <returns></returns>
        private IChillContainerInitializer BuildInitializer()
        {
            object attribute =
                GetContainerAttributeFromTestClass() ??
                GetContainerAttributeFromTestAssembly() ?? 
                new ChillContainerAttribute(typeof(AutofacChillContainer));

            Type type = ((ChillContainerInitializerAttribute) attribute).ChillContainerInitializerType;
            if (type == null)
            {
                throw new InvalidOperationException(
                    $"The type property on the {nameof(ChillContainerInitializerAttribute)} should not be null");
            }

            return (IChillContainerInitializer) Activator.CreateInstance(type);
        }

        private Attribute GetContainerAttributeFromTestClass()
        {
            return GetType()
                .GetTypeInfo()
                .GetCustomAttributes(typeof(ChillContainerInitializerAttribute), false)
                .OfType<Attribute>()
                .SingleOrDefault();
        }

        private Attribute GetContainerAttributeFromTestAssembly()
        {
            return GetType()
                .GetTypeInfo()
                .Assembly.GetCustomAttributes(typeof(ChillContainerInitializerAttribute))
                .SingleOrDefault();
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
        public T The<T>() where T : class
        {
            T value = Decorator.Get<T>();

            if (!Decorator.IsRegistered(typeof(T)))
            {
                Decorator.Set(value);
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
            var items = Decorator.Get<Dictionary<Tuple<Type, string>, object>>();
            var key = Tuple.Create(typeof(T), named);

            object item;
            if (!items.TryGetValue(key, out item))
            {
                item = Decorator.Get<T>(named);
                items.Add(key, item);
                decorator.Set(items);
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
            return decorator.AddToList(items);
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
            return decorator.AddToList(items.ToArray());
        }

        /// <summary>
        /// Returns all items registered in the chill container to a specific interface. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> All<T>() where T : class
        {
            return Decorator.GetList<T>();
        }

        /// <summary>
        /// Short hand for setting <see cref="The{T}"/> to a value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueToSet"></param>
        /// <returns></returns>
        public T UseThe<T>(T valueToSet) where T : class
        {
            return Decorator.Set(valueToSet);
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
            return Decorator.Set(valueToSet, named);
        }

        /// <summary>
        /// Cleans up all usages. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Clean up all instances created by the container. 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Decorator.Dispose();
            }
        }
    }
}
