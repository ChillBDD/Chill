using System;

namespace Chill
{
    /// <summary>
    /// Represents an object mother that is automatically discovered by Chill to construct test objects
    /// whenever you request one using <see cref="TestBase.The{T}"/>.
    /// </summary>
    public interface IObjectMother
    {
        /// <summary>
        /// Gets a value indicating whether this object mother is supposed to be used as a fallback in a case
        /// a more specific object mother is not available. 
        /// </summary>
        bool IsFallback { get; }

        /// <summary>
        /// Gets a value indicating whether this object mother can build objects of the specified <paramref name="type"/>. 
        /// </summary>
        bool Applies(Type type);

        /// <summary>
        /// Creates a test instance that is of, inherits from or implements the specified <paramref name="type"/>.
        /// </summary>
        /// <remarks>
        /// The implementation can use the provided <paramref name="objectResolver"/> to get other objects
        /// from either Chill's configured container or another object mother.    
        /// </remarks>
        object Create(Type type, IChillObjectResolver objectResolver);
    }

    /// <summary>
    /// Represents an object that allows resolving an object from the Chill container. 
    /// </summary>
    public interface IChillObjectResolver
    {
        /// <summary>
        /// Gets a value of the specified type from the container, optionally registered under a key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        T Get<T>(string key = null) where T : class;
    }
}