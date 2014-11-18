using Chill.AutofacNSubstitute;
using Chill.Tests.CoreScenarios;
using Chill.Tests.TestSubjects;
using FluentAssertions;

using NSubstitute;
using NSubstitute.Exceptions;

using Xunit;

namespace Chill.Tests
{


    public class AutoFacNSubstituteSpecs
    {
        [ChillContainer(typeof(AutofacNSubstituteChillContainer))]
        public class Given_Subject_WithAutofacNSubstitute : GivenSubjectSpecs
        {
            [Fact]
            public void Subject_is_not_generated_by_nsubstitute()
            {
                When(() => Subject.Received().DoSomething(), deferedExecution:true);
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

        [ChillContainer(typeof(AutofacNSubstituteChillContainer))]
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
}
