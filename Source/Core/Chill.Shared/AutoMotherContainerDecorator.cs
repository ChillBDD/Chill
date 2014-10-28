using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chill
{
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

        public void RegisterType<T>()
        {
            internalChillContainer.RegisterType<T>();
        }

        public T Get<T>(string key = null) where T : class
        {

            if (internalChillContainer.IsRegistered<T>())
            {
                return internalChillContainer.Get<T>();
            }

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
                return item;
                
            }

        }

        public T Set<T>(T valueToSet, string key = null) where T : class
        {
            return internalChillContainer.Set(valueToSet, key);
        }

        public void LoadAutoMothers(IEnumerable<Assembly> assemblies)
        {
            IEnumerable<Type> types = AssemblyTypeResolver.GetAllTypesFromAssemblies(assemblies)
                .Where(IsAutoMother);
            foreach (var type in types)
            {
                autoMothers.Add((IAutoMother)Activator.CreateInstance(type));
            }
        }

        private static bool IsAutoMother(Type x)
        {
#if WINRT
            return typeof (IAutoMother).GetTypeInfo().IsAssignableFrom(x.GetTypeInfo());
#else
            return typeof(IAutoMother).IsAssignableFrom(x) && !x.IsAbstract;
#endif

        }


        public bool IsRegistered<T>()
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Type type)
        {
            throw new NotImplementedException();
        }
    }
}