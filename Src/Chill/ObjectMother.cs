using System;

namespace Chill
{
    /// <summary>
    /// Convenience class for object mothers that construct a single type of object. 
    /// </summary>
    public abstract class ObjectMother<TTarget> : IObjectMother
    {
        /// <inheritdoc />
        public bool IsFallback => false;

        /// <inheritdoc />
        public bool Applies(Type type)
        {
            return type == typeof(TTarget);
        }

        /// <inheritdoc />
        public object Create(Type type, IChillObjectResolver container) 
        {
            if (!Applies(type))
            {
                throw new InvalidOperationException("ObjectMother only applies to ");
            }

            return Create();
        }

        /// <summary>
        /// Creates an instance of the requested type.
        /// </summary>
        /// <returns></returns>
        protected abstract TTarget Create();
    }
}    