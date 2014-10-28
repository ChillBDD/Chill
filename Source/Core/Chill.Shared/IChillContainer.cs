using System;

namespace Chill
{
    /// <summary>
    /// Represents an auto-mocking container to be used to build-up the subject-under-test as well as providing
    /// mock dependencies.
    /// </summary>
    public interface IChillContainer : IDisposable
    {
        void RegisterType<T>();

        T Get<T>(string key = null) where T : class;

        T Set<T>(T valueToSet, string key = null) where T : class;

        bool IsRegistered<T>();
        bool IsRegistered(Type type);
    }
}