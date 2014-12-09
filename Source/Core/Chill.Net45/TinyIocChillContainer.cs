using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;

namespace Chill
{
#if NET45

    public class TinyIocChillContainer : IChillContainer
    {
        private TinyIoCContainer _container;
        public TinyIocChillContainer()
        {
            _container = new TinyIoCContainer();
        }

        public TinyIocChillContainer(TinyIoCContainer container)
        {
            _container = container;
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
            return _container.Resolve<T>(key);
        }

        public T Set<T>(T valueToSet, string key = null) where T : class
        {
            _container.Register<T>(valueToSet, key);
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
#endif

}
