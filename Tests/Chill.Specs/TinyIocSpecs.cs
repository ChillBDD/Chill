using System;
using System.Collections.Generic;
using Chill.Tests.TestSubjects;
using FluentAssertions;
using Xunit;

namespace Chill.Tests.CoreScenarios
{
    public class BuiltInContainerSpecs
    {
        /// <summary>
        /// This only works in .net 4.5. If you don't need a full container, you can use the builtin tinyioc one. 
        /// </summary>
        public class When_using_the_builtin_container : GivenWhenThen
        {
            public When_using_the_builtin_container()
            {
                Given(() => SetThe<ITestService>().To(new TestService()));
            }

            /// <summary>
            /// Validate that the <see cref="AutofacChillContainerSpecs.AutofacContainerWithCustomModule" /> has registered the ITestService properly.
            /// </summary>
            [Fact]
            public void Then()
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
                Action a = () => The<ISomeInterface>();

                a.Should().Throw<Exception>();
            }
        }


        public interface ITestService
        {

        }

        public class TestService : ITestService
        {

        }

        public class DependentService
        {
            public DependentService(ITestService testService)
            {
                TestService = testService;
            }

            public ITestService TestService { get; }
        }
    }

    public interface ISomeInterface
    {
    }
}