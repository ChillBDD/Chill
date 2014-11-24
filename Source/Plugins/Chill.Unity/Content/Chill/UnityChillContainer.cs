using System.Collections.Generic;
using Autofac.Core;
using Chill;
using System;
using Microsoft.Practices.Unity;
namespace Chill.Unity
{
    internal class UnityChillContainer : IChillContainer
    {
        private readonly IUnityContainer _container;
        

        public UnityChillContainer()
            : this(new UnityContainer())
        {
        }


        public UnityChillContainer(IUnityContainer container)
        {
            _container = container;
            container.RegisterInstance(new Dictionary<Tuple<Type, string>, object>());
        }

        public IUnityContainer Container
        {
            get { return _container; }
        }

        public void Dispose()
        {
            Container.Dispose();
        }


        public void RegisterType<T>()
        {
            // No need to do anything for unity. It can resolve objects not registered. 
        }

        public T Get<T>(string key = null) where T : class
        {
            if (key == null)
            {
                return Container.Resolve<T>();
            }
            else
            {
                return Container.Resolve<T>(key);
            }
        }

        public T Set<T>(T valueToSet, string key = null) where T : class
        {
            if (key == null)
            {
                Container.RegisterInstance(valueToSet);

            }
            else
            {
                Container.RegisterInstance(key, valueToSet);
            }
            return Get<T>();
        }


        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof(T));
        }

        public bool IsRegistered(System.Type type)
        {
            return Container.IsRegistered(type);
        }
    }
}