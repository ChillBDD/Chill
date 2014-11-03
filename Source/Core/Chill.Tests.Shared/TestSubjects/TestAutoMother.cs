using System;

namespace Chill.Tests.TestSubjects
{
    public class TestAutoMother : IAutoMother
    {
        public bool Applies(Type type)
        {
            return type == typeof (Subject_built_By_Chill_AutoMother);
        }

        public T Create<T>(IChillContainer container)
        {
            if (typeof (T) != typeof (Subject_built_By_Chill_AutoMother))
            {
                throw new InvalidOperationException("This builder can only build Subject_built_By_Chill_AutoMother");
            }

            return (T)(object) new Subject_built_By_Chill_AutoMother("I have been built by Chill");
        }
    }
}