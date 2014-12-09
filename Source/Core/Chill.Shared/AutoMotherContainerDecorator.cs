using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chill
{
    /// <summary>
    /// Wraps an <see cref="IChillContainer"/> to add AutoMother decorating capabilities. 
    /// </summary>
    public class AutoMotherContainerDecorator : IChillContainer
    {
        private IChillContainer internalChillContainer;
        private Dictionary<Tuple<Type, string>, object> initializedValues = new Dictionary<Tuple<Type, string>, object>();
        private List<IAutoMother> autoMothers = new List<IAutoMother>();
        private object syncRoot = new object();


        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMotherContainerDecorator"/> class.
        /// </summary>
        /// <param name="internalChillContainer">The internal chill container.</param>
        public AutoMotherContainerDecorator(IChillContainer internalChillContainer)
        {
            this.internalChillContainer = internalChillContainer;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            internalChillContainer.Dispose();
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterType<T>() where T : class
        {
            internalChillContainer.RegisterType<T>();
        }

        /// <summary>
        /// Gets an instance from the container, optionally by key.
        /// </summary>
        /// <typeparam name="T">The type of object registered in the container. </typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The requested value from the container</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if more than one builder of the specified type is registered</exception>
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

        /// <summary>
        /// Sets the specified value to set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueToSet">The value to set.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Set<T>(T valueToSet, string key = null) where T : class
        {
            return internalChillContainer.Set(valueToSet, key);
        }


        /// <summary>
        /// Loads the automatic mothers.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
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

        /// <summary>
        /// Determines whether an instance of this type is registered.
        /// </summary>
        /// <returns></returns>
        public bool IsRegistered<T>()  where T : class
        {
            return internalChillContainer.IsRegistered<T>();
        }


        /// <summary>
        /// Determines whether an instance of this type is registered.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public bool IsRegistered(Type type)
        {
            return internalChillContainer.IsRegistered(type);

        }
    }
}