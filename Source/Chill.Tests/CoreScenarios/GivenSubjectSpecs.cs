using Chill.Tests.TestSubjects;

using FluentAssertions;

using Xunit;

namespace Chill.Tests.CoreScenarios
{
    public abstract class GivenSubjectSpecs : GivenSubject<TestSubject>
    {
        [Fact]
        public void Then_subject_gets_dependencies_injected()
        {
            Subject.TestService.Should().NotBeNull();
        }

        [Fact]
        public void When_configuring_mock_in_given_then_injected_mock_should_be_same()
        {
            ITestService testService = null;
            Given(() => testService = The<ITestService>());

            Subject.TestService.Should().BeSameAs(testService);
        }

        [Fact]
        public void When_getting_a_mock_twice_should_return_same_instance()
        {
            The<ITestService>().Should().BeSameAs(The<ITestService>());
        }

        [Fact]
        public void Can_get_mock_for_subject()
        {
            The<ITestService>().Should().NotBeNull();
        }

        [Fact]
        public void Can_get_named_service()
        {
            TheNamed<ITestService>("abc").Should().NotBeNull();
        }
    }
}