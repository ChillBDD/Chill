using System.Collections.Generic;
using System.Reflection;
using System;

namespace Chill
{
    /// <summary>
    /// Interface that will help 
    /// </summary>
    public interface IChillTestInitializer
    {
        IEnumerable<Assembly> FindRelevantAssemblies(TestBase test);

        IChillContainer BuildChillContainer(TestBase test);

        void InitializeContainer(TestBase test);
    }
}