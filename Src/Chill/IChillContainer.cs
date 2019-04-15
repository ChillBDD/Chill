using System;

namespace Chill
{
    /// <summary>
    /// Represents a container to be used to build-up the subject-under-test as well as providing
    /// any dependencies.
    /// </summary>
    public interface IChillContainer : IDisposable
    {
        /// <summary>
        /// Registers a concrete type at the container. 
        /// </summary>
        /// <remarks>
        /// For example, Autofac cannot create objects until you register them.
        /// </remarks>
        void RegisterType<T>() where T : class;

        /// <summary>
        /// Gets a instance of the specified <typeparamref name="T"/> from the container, optionally
        /// registered under <paramref name="key"/>.
        /// </summary>
        /// <returns>
        /// Returns an instance or implementation of the registered type or <c>null</c> if no such type exists in the container. 
        /// </returns>
        T Get<T>(string key = null) where T : class;

        /// <summary>
        /// Sets a value in the container, so that from now on, it will be returned when you call <see cref="Get{T}"/>
        /// </summary>
        T Set<T>(T valueToSet, string key = null) where T : class;

        /// <summary>
        /// Determines whether an instance of this type is currently .
        /// </summary>
        bool IsRegistered(Type type);
    }

}