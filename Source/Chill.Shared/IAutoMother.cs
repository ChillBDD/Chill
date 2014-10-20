using System;

namespace Chill
{
    public interface IAutoMother
    {
        bool Applies(Type type);

        T Create<T>(IChillContainer container);
    }
}