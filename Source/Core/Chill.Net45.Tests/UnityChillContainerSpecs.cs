using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Chill.Autofac;
using Chill.AutofacNSubstitute;
using Chill.Unity;
using Chill.UnityNSubstitute;
using Microsoft.Practices.Unity;
using NSubstitute;
using NSubstitute.Exceptions;
using Xunit;
using FluentAssertions;
using Chill.Tests.TestSubjects;
using Xunit.Extensions;

namespace Chill.Tests.CoreScenarios
{
    public class UnityNSubstituteSpecs
    {
        [ChillContainer(typeof(UnityNSubstituteChillContainer))]
        public class Given_Subject_WithAutofacNSubstitute : GivenSubjectSpecs
        {
            [Fact]
            public void Subject_is_not_generated_by_nsubstitute()
            {
                When(() => Subject.Received().DoSomething(), deferedExecution: true);
                WhenAction.ShouldThrow<NotASubstituteException>();
            }

            [Fact]
            public void Then_mocks_are_generated_by_nsubstitute()
            {
                The<ITestService>().TryMe().Returns(true);
                The<ITestService>().TryMe().Should().BeTrue();
                The<ITestService>().Received().TryMe();
            }

            [Fact]
            public void Subject_gets_mock_injected()
            {
                The<ITestService>().TryMe().Returns(true);
                Subject.DoSomething().Should().BeTrue();
                The<ITestService>().Received().TryMe();
            }
        }

        [ChillContainer(typeof(UnityNSubstituteChillContainer))]
        public class Given_TestBase_With_WithAutofacNSubstitute : TestBaseSpecs
        {
            [Fact]
            public void Then_mocks_are_generated_by_nsubstitute()
            {
                The<ITestService>().TryMe().Returns(true);
                The<ITestService>().TryMe().Should().BeTrue();
                The<ITestService>().Received().TryMe();
            }
        }
    }

    public class UnityChillContainerSpecs
    {
        /// <summary>
        /// This test checks if you can use Chill as an integration test. It sets up a regular unity container
        /// container (without mocking) <see cref="UnityContainerWithCustomModule"/>. 
        /// </summary>
        [ChillContainer(typeof(UnityContainerWithCustomModule))]
        public class When_configuring_container_with_module_and_objectmother : GivenWhenThen
        {
            /// <summary>
            /// Validate that the <see cref="UnityContainerWithCustomModule" /> has registered the ITestService properly.
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

            [Theory, InlineData(), InlineData()]
            public void Then_singleton_is_only_resolved_once()
            {
                The<TestSingleton>().GetInstanceCount().Should().Be(1);
            }

            /// <summary>
            /// Any types not explicitly registered in the container should not be resolvable. 
            /// </summary>
            [Fact]
            public void Then_cannot_resolve_unknown_interface()
            {
                Action a = () => The<IAppDomainSetup>();

                a.ShouldThrow<ResolutionFailedException>()
                    .And.Message.Should().Contain("Are you missing a type mapping?");
            }
        }


        /// <summary>
        /// You can define 'non mocking' with all the type registrations that you would otherwise in your application. 
        /// This can either be done in the App. This class also creates a single 'parent' container and then child containers
        /// per test. 
        /// </summary>
        internal class UnityContainerWithCustomModule : UnityChillContainer
        {
            private static IUnityContainer staticContainer;
            private static object syncRoot = new object();
            public UnityContainerWithCustomModule()
                : base(CreateContainer())
            {

            }
            /// <summary>
            /// This method creates the Autofac container and registers the custom type 
            /// </summary>
            /// <returns></returns>
            private static IUnityContainer CreateContainer()
            {
                if (staticContainer == null)
                {
                    lock (syncRoot)
                    {
                        if (staticContainer == null)
                        {
                            
                            staticContainer = new UnityContainer();
                            staticContainer.AddNewExtension<CustomUnityExtension>();
                        }
                    }
                }
                return staticContainer.CreateChildContainer();
            }
        }

        public class CustomUnityExtension : UnityContainerExtension
        {

            protected override void Initialize()
            {
                Container.RegisterType<ITestService, TestService>();
                Container.RegisterType<TestSingleton>(new ContainerControlledLifetimeManager());
            }
        }

        public interface ITestService
        {

        }
        public class TestService : ITestService
        {

        }

        public class TestSingleton
        {
            public TestSingleton()
            {
                Interlocked.Increment(ref _instanceCount);
            }
            private static int _instanceCount;

            public int GetInstanceCount()
            {
                return _instanceCount;
            }
        }

    }

}
