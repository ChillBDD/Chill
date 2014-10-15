using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Chill
{
    public interface IChillTestInitializer
    {
        IEnumerable<Assembly> FindRelevantAssemblies(TestBase test);

        IChillContainer BuildChillContainer();

        void InitializeContainer(TestBase test);

        T Get<T>(TestBase test, string key = null) where T : class;
        T Set<T>(TestBase test, T valueToSet, string key = null) where T : class;

    }

    public class DefaultChillTestInitializer<TContainerType> : IChillTestInitializer
        where TContainerType : IChillContainer, new()
    {
        private AutoMotherContainerDecorator _container;


        public virtual IEnumerable<Assembly> FindRelevantAssemblies(TestBase test)
        {
            yield return test.GetType().Assembly;
        }

        public virtual IChillContainer BuildChillContainer()
        {
            return _container =  new AutoMotherContainerDecorator(new TContainerType());
        }

        public virtual void InitializeContainer(TestBase test)
        {
            _container.LoadTypeBuilders(FindRelevantAssemblies(test));
        }

        public T Get<T>(TestBase test, string key = null) where T : class
        {
            return _container.Get<T>(key);
        }

        public T Set<T>(TestBase test, T valueToSet, string key = null) where T : class
        {
            return _container.Set(valueToSet, key);
        }
    }

    public class AutoMotherContainerDecorator : IChillContainer
    {
        private IChillContainer internalChillContainer;
        private Dictionary<Tuple<Type, string>, object> initializedValues = new Dictionary<Tuple<Type, string>, object>();
        private List<IAutoMother> autoMothers = new List<IAutoMother>();
        private object syncRoot = new object();
        public AutoMotherContainerDecorator(IChillContainer internalChillContainer)
        {
            this.internalChillContainer = internalChillContainer;
        }

        public void Dispose()
        {
            internalChillContainer.Dispose();
        }

        public T Get<T>(string key = null) where T : class
        {
            // Combine the type and key into a string
            var initializedValuesKey = new Tuple<Type, string>(typeof (T), key);
            if (initializedValues.ContainsKey(initializedValuesKey))
                return initializedValues[initializedValuesKey] as T;

            lock (syncRoot)
            {
                if (initializedValues.ContainsKey(initializedValuesKey))
                    return initializedValues[initializedValuesKey] as T;

                var applicableMothers = autoMothers.Where(x => x.Applies(typeof(T))).ToList();

                if (!applicableMothers.Any())
                {
                    return internalChillContainer.Get<T>(key);
                }
                if (applicableMothers.Count > 1)
                {
                    throw new InvalidOperationException(string.Format("There are more than one builders that apply to build: {0}. Namely: {1}.", typeof(T).Name, string.Join(",", applicableMothers.Select(x => x.GetType().Name))));
                }
                var item = applicableMothers.First().Create<T>(internalChillContainer);
                initializedValues.Add(initializedValuesKey, item);
                Set(item, key);
                return item;
                
            }

        }

        public T Set<T>(T valueToSet, string key = null) where T : class
        {
            return internalChillContainer.Set(valueToSet, key);
        }

        public void LoadTypeBuilders(IEnumerable<Assembly> assemblies)
        {
            var types = AssemblyTypeResolver.GetAllTypesFromAssemblies(assemblies)
                .Where(x => typeof(IAutoMother).IsAssignableFrom(x));

            foreach (var type in types)
            {
                autoMothers.Add((IAutoMother)Activator.CreateInstance(type));
            }

        }
    }
}