using System;
using System.Collections.Generic;
using System.Reflection;

namespace Chill
{
    /// <summary>
    /// Default implementation of a Chill Test Initializer. This implementation will create 
    /// container of TContainerType and wrap it in a 
    /// <see cref="ChillContainerExtensions"/>. This way, you can create automatic mothers 
    /// for your classes. 
    /// </summary>
    /// <typeparam name="TContainerType">The type of the container to create. </typeparam>
    public class DefaultChillContainerInitializer<TContainerType> : IChillContainerInitializer
        where TContainerType : IChillContainer, new()
    {
        private IChillContainer container;

        /// <summary>
        /// Build the Chill Container. This container will be wrapped in a AutoMotherContainerDecorator. 
        /// </summary>
        /// <returns></returns>
        public virtual IChillContainer BuildChillContainer(TestBase testBase)
        {
            return container = Activator.CreateInstance<TContainerType>();
        }

        /// <summary>
        /// Initialize the container. The base implementation will load the AutoMothers from the relevant assemblies. 
        /// 
        /// Override this method to provide further customization of the container. 
        /// </summary>
        /// <param name="test"></param>
        public virtual void InitializeContainer(TestBase test)
        {
            if (container != null)
            {
                container.RegisterType<Dictionary<Type, object>>();
                container.RegisterType<Dictionary<Tuple<Type, string>, object>>();
            }
        }
    }
}