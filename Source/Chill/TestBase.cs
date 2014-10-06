using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chill
{
    /// <summary>
    /// Base class for tests that use a similar appraoch. 
    /// </summary>
    public abstract class TestBase: IDisposable
    {
        protected bool ExpectExceptions;
        protected Exception CaughtException;
        public IAutoMockingContainer Container { get; private set; }
        protected virtual void OnWhen()
        {
            try
            {
                TriggerWhen().Wait();
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

        protected virtual Task TriggerWhen()
        {
            return Task.Run(() => { });
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


    public interface IStoreStateBuilder<T>
    {
        TestBase TestBase { get; set; }

        TestBase To(T valueToSet);

    }

    public class StoreStateBuilder<T> : IStoreStateBuilder<T>
        where T: class
    {
        public StoreStateBuilder(TestBase testBase)
        {
            TestBase = testBase;
        }


        public virtual TestBase To(T valueToSet)
        {
            TestBase.Container.Set(valueToSet);
            return TestBase;
        }


        public TestBase TestBase { get; set; }
    }

    public class IndexedStoredStateBuilder<T> : StoreStateBuilder<T> where T : class
    {
        public int Index { get; private set; }

        public IndexedStoredStateBuilder(TestBase testBase, int index) : base(testBase)
        {
            Index = index;
        }

        public override TestBase To(T valueToSet)
        {
            var list = TestBase.Container.Get<List<T>>();

            if (list == null)
            {
                list = new List<T>();
            }

            while (list.Count < this.Index)
            {
                list.Add(default(T));
            }

            list.Add(valueToSet);
            TestBase.Container.Set(list);


            return TestBase;
        }
    }

    public static class StoreStateBuilderExtensions
    {
        public static IndexedStoredStateBuilder<T> AtIndex<T>(this IStoreStateBuilder<T> subject, int index) where T : class
        {
            return new IndexedStoredStateBuilder<T>(subject.TestBase, index);
        }
    }
}
