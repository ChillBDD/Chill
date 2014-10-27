using System;

namespace Chill
{
    public interface IAutoMother
    {
        bool Applies(Type type);

        T Create<T>(IChillContainer container);
    }

    public abstract class ObjectMother<TTarget> : IAutoMother
    {
        public bool Applies(Type type)
        {
            return type == typeof(TTarget);
        }

        public T Create<T>(IChillContainer container)
        {
            if (!Applies(typeof(T)))
            {
                throw new InvalidOperationException("ServerMother only applies to ");
            }
            return (T)(object)Create();
        }

        protected abstract TTarget Create();

    }

}