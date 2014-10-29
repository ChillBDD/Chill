using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using NSubstitute;

namespace Chill.Autofac
{
    internal class AutofacNSubstituteChillContainer : AutofacChillContainer
    {

        public AutofacNSubstituteChillContainer()
            : base(CreateBuilder())
        {
        }

        static ContainerBuilder CreateBuilder()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterSource(new NSubstituteRegistrationHandler());
            return containerBuilder;
        }

        /// <summary> Resolves unknown interfaces and Mocks using the <see cref="Substitute"/>. </summary>
        internal class NSubstituteRegistrationHandler : IRegistrationSource
        {
            /// <summary>
            /// Retrieve a registration for an unregistered service, to be used
            /// by the container.
            /// </summary>
            /// <param name="service">The service that was requested.</param>
            /// <param name="registrationAccessor"></param>
            /// <returns>
            /// Registrations for the service.
            /// </returns>
            public IEnumerable<IComponentRegistration> RegistrationsFor
                (Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
            {
                if (service == null)
                    throw new ArgumentNullException("service");

                var typedService = service as IServiceWithType;
                if (typedService == null ||
                    !typedService.ServiceType.IsInterface ||
                    typedService.ServiceType.IsGenericType &&
                    typedService.ServiceType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                    typedService.ServiceType.IsArray ||
                    typeof(IStartable).IsAssignableFrom(typedService.ServiceType))
                    return Enumerable.Empty<IComponentRegistration>();

                var rb = RegistrationBuilder.ForDelegate((c, p) => Substitute.For(new[] { typedService.ServiceType }, null))
                    .As(service)
                    .InstancePerLifetimeScope();

                return new[] { rb.CreateRegistration() };
            }

            public bool IsAdapterForIndividualComponents
            {
                get { return false; }
            }
        }
    }
}