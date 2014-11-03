using System;

namespace Chill
{
    /// <summary>
    /// Base class for object mothers that construct a single object. 
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    public abstract class ObjectMother<TTarget> : IAutoMother
    {
        /// <summary>
        /// Checks if the specified type applies.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public bool Applies(Type type)
        {
            return type == typeof(TTarget);
        }

        /// <summary>
        /// Creates the specified type using the specified container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">ServerMother only applies to </exception>
        public T Create<T>(IChillContainer container)
        {
            if (!Applies(typeof(T)))
            {
                throw new InvalidOperationException("ServerMother only applies to ");
            }
            return (T)(object)Create();
        }

        /// <summary>
        /// Creates an instance of the requested type.
        /// </summary>
        /// <returns></returns>
        protected abstract TTarget Create();

    }
}