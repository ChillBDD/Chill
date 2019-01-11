using System;
using System.Collections.Generic;
using System.Reflection;

namespace Chill
{
    /// <summary>
    /// Default implementation of a Chill Test Initializer. This implementation will create 
    /// container of TContainerType and wrap it in a 
    /// <see cref="AutoMockingContainerExtensions"/>. This way, you can create automatic mothers 
    /// for your classes. 
    /// </summary>
    /// <typeparam name="TContainerType">The type of the container to create. </typeparam>
    public class DefaultChillContainerInitializer<TContainerType> : IChillContainerInitializer
        where TContainerType : IChillContainer, new()
    {

        private AutoMotherContainerDecorator _container;

        /// <summary>
        /// Find all relevant assemblies that should be scanned (for example for AutoMothers). 
        /// </summary>
        /// <param name="test">The test object to use.</param>
        /// <returns>List of assemblies to scan.</returns>
        public virtual IEnumerable<Assembly> FindRelevantAssemblies(TestBase test)
        {
            yield return test.GetType().GetTypeInfo().Assembly;

        }

        /// <summary>
        /// Build the Chill Container. This container will be wrapped in a AutoMotherContainerDecorator. 
        /// </summary>
        /// <returns></returns>
        public virtual IChillContainer BuildChillContainer(TestBase testBase)
        {
            return _container =  new AutoMotherContainerDecorator(testBase.BuildContainer(typeof(TContainerType)));
        }

        /// <summary>
        /// Initialize the container. The base implementation will load the AutoMothers from the relevant assemblies. 
        /// 
        /// Override this method to provide further customization of the container. 
        /// </summary>
        /// <param name="test"></param>
        public virtual void InitializeContainer(TestBase test)
        {
            if (_container != null)
            {
                _container.RegisterType<Dictionary<Type, object>>();
                _container.RegisterType<Dictionary<Tuple<Type, string>, object>>();
                _container.LoadAutoMothers(FindRelevantAssemblies(test));
            }
        }
    }
}