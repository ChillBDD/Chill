using System;

namespace Chill
{
    /// <summary>
    /// Represents an auto-mocking container to be used to build-up the subject-under-test as well as providing
    /// mock dependencies.
    /// </summary>
    /// <remarks>
    /// The implementation must allow creating instances of unregistered concrete types. 
    /// </remarks>
    public interface IChillContainer : IDisposable
    {
        /// <summary>
        /// Registers a type to the container. For example, autofac cannot create objects until you register them
        /// This allows containers like autofac to create the object. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RegisterType<T>() where T : class;

        /// <summary>
        /// Gets a value of the specified type from the container, optionally registered under a key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        T Get<T>(string key = null) where T : class;

        /// <summary>
        /// Sets a value in the container, so that from now on, it will be returned when you call <see cref="Get{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueToSet">The value to set.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        T Set<T>(T valueToSet, string key = null) where T : class;

        /// <summary>
        /// Determines whether an instance of this type is registered.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        bool IsRegistered(Type type);
    }

}