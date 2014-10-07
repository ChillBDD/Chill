using Autofac.Extras.FakeItEasy;

namespace Chill.AutofacFakeItEasy
{
    /// <summary>
    /// An implementation of <see cref="IAutoMockingContainer"/> that uses Autofac and FakeItEasy to build objects
    /// with mocked dependencies.
    /// </summary>
    public class AutofacFakeItEasyMockingContainer : IAutoMockingContainer
    {
        private readonly AutoFake autoFake = new AutoFake();

        public T Get<T>() where T : class
        {
            return autoFake.Resolve<T>();
        }

        public T Set<T>(T valueToSet) where T : class
        {
            return autoFake.Provide(valueToSet);
        }

        public void Dispose()
        {
            autoFake.Dispose();
        }
    }
}