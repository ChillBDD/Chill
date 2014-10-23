using Autofac;
using Autofac.Extras.FakeItEasy;
using Chill;

using NSubstitute;

namespace Chill.Autofac
{
    /// <summary>
    /// An implementation of <see cref="IAutoMockingContainer"/> that uses Autofac and FakeItEasy to build objects
    /// with mocked dependencies.
    /// </summary>
    internal class AutofacFakeItEasyMockingContainer : AutofacChillContainer
    {
        public AutofacFakeItEasyMockingContainer()
            : base(CreateContainerBuilder())
        {

        }

        private static ContainerBuilder CreateContainerBuilder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new FakeRegistrationHandler(false, false, false, null));
            return builder;
        }
    }
}