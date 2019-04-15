using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chill
{
    /// <summary>
    ///     Wraps an <see cref="IChillContainer" /> to allow implementations of <see cref="IObjectMother"/> to
    /// generate objects not registered in the container. 
    /// </summary>
    public class ObjectMotherContainerDecorator : IChillContainer, IChillObjectResolver
    {
        private readonly List<IObjectMother> objectMothers = new List<IObjectMother>();

        private readonly Dictionary<Tuple<Type, string>, object>
            initializedValues = new Dictionary<Tuple<Type, string>, object>();

        private readonly IChillContainer internalChillContainer;
        private readonly object syncRoot = new object();


        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectMotherContainerDecorator" /> class.
        /// </summary>
        /// <param name="internalChillContainer">The internal chill container.</param>
        public ObjectMotherContainerDecorator(IChillContainer internalChillContainer)
        {
            this.internalChillContainer = internalChillContainer;
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            internalChillContainer.Dispose();
        }

        /// <summary>
        ///     Registers the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterType<T>() where T : class
        {
            internalChillContainer.RegisterType<T>();
        }

        /// <summary>
        ///     Gets an instance from the container, optionally by key.
        /// </summary>
        /// <typeparam name="T">The type of object registered in the container. </typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The requested value from the container</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if more than one builder of the specified type is registered</exception>
        public T Get<T>(string key = null) where T : class
        {
            if (internalChillContainer.IsRegistered(typeof(T)))
            {
                return internalChillContainer.Get<T>();
            }

            // Combine the type and key into a string
            var initializedValuesKey = new Tuple<Type, string>(typeof(T), key);
            if (initializedValues.ContainsKey(initializedValuesKey))
            {
                return initializedValues[initializedValuesKey] as T;
            }

            lock (syncRoot)
            {
                if (initializedValues.ContainsKey(initializedValuesKey))
                {
                    return initializedValues[initializedValuesKey] as T;
                }

                var applicableMothers = objectMothers
                    .Where(m => m.Applies(typeof(T)))
                    .OrderBy(m => m.IsFallback)
                    .ToList();
                
                if (!applicableMothers.Any())
                {
                    return internalChillContainer.Get<T>(key);
                }

                if (applicableMothers.Count(m => !m.IsFallback) > 1)
                {
                    throw new InvalidOperationException(
                        $"There are more than one builders that apply to build: {typeof(T).Name}. Namely: {string.Join(",", applicableMothers.Select(x => x.GetType().Name))}.");
                }

                object item = applicableMothers
                    .First()
                    .Create(typeof(T), new ContainerResolverAdapter(internalChillContainer));
                
                initializedValues.Add(initializedValuesKey, item);
                return (T) item;
            }
        }

        /// <summary>
        ///     Sets the specified value to set.
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
        ///     Determines whether an instance of this type is registered.
        /// </summary>
        /// <returns></returns>
        public bool IsRegistered<T>() where T : class
        {
            return internalChillContainer.IsRegistered(typeof(T));
        }


        /// <summary>
        ///     Determines whether an instance of this type is registered.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public bool IsRegistered(Type type)
        {
            return internalChillContainer.IsRegistered(type);
        }

        /// <summary>
        ///     Loads the automatic mothers.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public void LoadAutoMothers(IEnumerable<Assembly> assemblies)
        {
            var types = AssemblyTypeResolver
                .GetAllTypesFromAssemblies(assemblies)
                .Where(IsAutoMother);

            foreach (var type in types)
            {
                objectMothers.Add((IObjectMother) Activator.CreateInstance(type));
            }
        }

        private static bool IsAutoMother(Type x)
        {
            return typeof(IObjectMother).GetTypeInfo().IsAssignableFrom(x.GetTypeInfo());
        }
    }

    internal class ContainerResolverAdapter : IChillObjectResolver
    {
        private readonly IChillContainer container;

        public ContainerResolverAdapter(IChillContainer container)
        {
            this.container = container;
        }

        public T Get<T>(string key = null) where T : class
        {
            return container.Get<T>(key);
        }
    }
}