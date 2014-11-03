using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using Chill.Tests.TestSubjects;
using Chill.Autofac;
using Autofac.Core.Registration;
using Autofac;

namespace Chill.Tests.CoreScenarios
{
    public class AutofacChillContainerSpecs
    {
        /// <summary>
        /// This test checks if you can use Chill as an integration test. It sets up a regular autofac
        /// container (without mocking) using an autofac module <see cref="AutofacContainerWithCustomModule"/>. 
        /// </summary>
        [ChillContainer(typeof(AutofacContainerWithCustomModule))]
        public class When_configuring_container_with_module_and_objectmother : GivenWhenThen
        {
            /// <summary>
            /// Validate that the <see cref="AutofacContainerWithCustomModule" /> has registered the ITestService properly.
            /// </summary>
            [Fact]
            public void Then_testservice_is_registered_through_module()
            {
                The<ITestService>().GetType().Should().Be(typeof(TestService));
            }

            /// <summary>
            /// The Subject_built_By_Chill_AutoMother should be constructed by an automother. 
            /// </summary>
            [Fact]
            public void Then_customer_is_resolved_through_Customer_Automother()
            {
                The<Subject_built_By_Chill_AutoMother>().Name.Should().NotBeNull();
            }

            /// <summary>
            /// Any types not explicitly registered in the container should not be resolvable. 
            /// </summary>
            [Fact]
            public void Then_cannot_resolve_unknown_interface()
            {
                Action a = () => The<IAppDomainSetup>();

                a.ShouldThrow<ComponentNotRegisteredException>();
            }
        }


        /// <summary>
        /// You can define 'non mocking' with all the type registrations that you would otherwise in your application. 
        /// This can either be done in the App
        /// </summary>
        internal class AutofacContainerWithCustomModule : AutofacChillContainer
        {
            public AutofacContainerWithCustomModule()
                : base(CreateContainerBuilder())
            {

            }
            /// <summary>
            /// This method creates the Autofac container and registers the custom type 
            /// </summary>
            /// <returns></returns>
            private static ContainerBuilder CreateContainerBuilder()
            {
                var builder = new ContainerBuilder();
                builder.RegisterModule<CustomAutofacModule>();
                return builder;
            }
        }



        public class CustomAutofacModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<TestService>().As<ITestService>();
                base.Load(builder);
            }
        }

        public interface ITestService
        {

        }
        public class TestService : ITestService
        {

        }

    }

}
