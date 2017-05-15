using System;

using TinyIoC;

namespace Chill
{
    public class TinyIocChillContainer : IChillContainer
    {
        private readonly TinyIoCContainer _container;

        public TinyIocChillContainer()
        {
            _container = new TinyIoCContainer();
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public void RegisterType<T>() where T : class
        {
            _container.Register<T>();
        }

        public T Get<T>(string key = null) where T : class
        {
            return _container.Resolve<T>(key ?? string.Empty);
        }

        public T Set<T>(T valueToSet, string key = null) where T : class
        {
            _container.Register(valueToSet, key ?? string.Empty);
            return valueToSet;
        }

        public bool IsRegistered<T>() where T : class
        {
            return _container.CanResolve<T>(ResolveOptions.FailUnregisteredAndNameNotFound);
        }

        public bool IsRegistered(Type type)
        {
            return _container.CanResolve(type);
        }
    }
}
