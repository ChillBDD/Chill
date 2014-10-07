using System;

namespace Chill
{
    /// <summary>
    /// Represents an auto-mocking container to be used to build-up the subject-under-test as well as providing
    /// mock dependencies.
    /// </summary>
    public interface IAutoMockingContainer : IDisposable
    {
        T Get<T>() where T : class;

        T Set<T>(T valueToSet) where T : class;
    }
}