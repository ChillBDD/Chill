using System.Collections.Generic;
using System.Reflection;
using System;

namespace Chill
{
    /// <summary>
    /// Interface for classes that initialize chill containers. 
    /// </summary>
    public interface IChillContainerInitializer
    {
        /// <summary>
        /// Builds the chill container.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <returns></returns>
        IChillContainer BuildChillContainer(TestBase test);

        /// <summary>
        /// Initializes the container.
        /// </summary>
        /// <param name="test">The test.</param>
        void InitializeContainer(TestBase test);
    }
}