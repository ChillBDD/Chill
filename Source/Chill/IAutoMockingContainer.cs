using System;

namespace Chill
{
    public interface IAutoMockingContainer : IDisposable
    {
        T Get<T>()
            where T:class;

        T Set<T>(T valueToSet)
            where T:class;
    }
}