using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Chill.Autofac;
using Chill.Tests.CoreScenarios;
using Chill.Tests.TestSubjects;
using FakeItEasy;
using FakeItEasy.Configuration;

using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Chill.Tests
{
    public class AutofacFakeItEasySpec
    {


        [ChillTestInitializer(typeof(DefaultChillTestInitializer<AutofacFakeItEasyMockingContainer>))]
        public class Given_Subject_WithAutofacNSubstitute : GivenSubjectSpecs
        {
            [Fact]
            public void Subject_is_not_generated_by_fakeItEasy()
            {
                When(() => A.CallTo(() => Subject.DoSomething()).Returns(true), deferedExecution: true);
                WhenAction.ShouldThrow<FakeConfigurationException>();
            }

            [Fact]
            public void Can_get_FakeItEasy_mock_from_container()
            {
                var testService = The<ITestService>();
                A.CallTo(() => The<ITestService>().TryMe()).Returns(true);

                The<ITestService>().TryMe().Should().Be(true);
            }

            [Fact]
            public void Subject_gets_mock_injected()
            {
                A.CallTo(() => The<ITestService>().TryMe()).Returns(true);

                Subject.DoSomething().Should().BeTrue();
                A.CallTo(() => The<ITestService>().TryMe()).MustHaveHappened();
            }

        }

        [ChillTestInitializer(typeof(DefaultChillTestInitializer<AutofacFakeItEasyMockingContainer>))]
        public class Given_TestBase_With_WithAutofacNSubstitute : TestBaseSpecs
        {
            [Fact]
            public void Then_mocks_are_generated_by_FakeItEasy()
            {
                A.CallTo(() => The<ITestService>().TryMe()).Returns(true);
                The<ITestService>().TryMe().Should().BeTrue();
                A.CallTo(() => The<ITestService>().TryMe()).MustHaveHappened();
            }
        }
    }


}
