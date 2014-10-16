using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Chill
{
    /// <summary>
    /// Interface that will help 
    /// </summary>
    public interface IChillTestInitializer
    {
        IEnumerable<Assembly> FindRelevantAssemblies(TestBase test);

        IChillContainer BuildChillContainer();

        void InitializeContainer(TestBase test);
    }
}