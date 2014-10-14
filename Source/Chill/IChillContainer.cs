using System;

namespace Chill
{
    /// <summary>
    /// Represents an auto-mocking container to be used to build-up the subject-under-test as well as providing
    /// mock dependencies.
    /// </summary>
    public interface IChillContainer : IDisposable
    {
        T Get<T>() where T : class;

        T Set<T>(T valueToSet) where T : class;
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
    public class ChillContainerAttribute : Attribute
    {
        public Type ChillContainerType { get; private set; }

        public ChillContainerAttribute(Type chillContainerType)
        {
            ChillContainerType = chillContainerType;
        }
    }
}