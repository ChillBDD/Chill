using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Extras.FakeItEasy;
using Chill;

    /// <summary>
    /// An implementation of <see cref="IAutoMockingContainer"/> that uses Autofac and FakeItEasy to build objects
    /// with mocked dependencies.
    /// </summary>
    internal class AutofacFakeItEasyMockingContainer : IChillContainer
    {
        private readonly AutoFake autoFake = new AutoFake();

        public T Get<T>(string key = null) where T : class
        {
            if (key == null)
            {
                return autoFake.Resolve<T>();
            }
            else
            {
                return autoFake.Container.ResolveKeyed<T>(key);
            }
        }

        public T Set<T>(T valueToSet, string key = null) where T : class
        {
            if (key == null)
            {
                return autoFake.Provide(valueToSet);
            }
            else
            {
                autoFake.Container.ComponentRegistry
                    .Register(RegistrationBuilder.ForDelegate((c, p) => valueToSet)
                        .As(new KeyedService(key, typeof (T)))
                        .InstancePerLifetimeScope().CreateRegistration());
                return this.autoFake.Container.ResolveKeyed<T>(key);

            }
        }

        public void Dispose()
        {
            autoFake.Dispose();
        }
    }
