using System.Threading;
using System.Threading.Tasks;

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

        [Fact]
        public void Given_is_executed_before_when()
        {
            string message = "";
            Given(() => message += "given");
            Given(() => message += "given");
            When(() => message+= "when");
            message.Should().Be("givengivenwhen");
        }

        [Fact]
        public void Can_Defer_when()
        {
            string message = "";
            Given(() => message += "given");
            When(() => message += "when", deferedExecution:true);
            message.Should().Be("given");
            WhenAction().Wait();
            message.Should().Be("givenwhen");
        }

        [Fact]
        public void Can_Defer_when_with_property()
        {
            this.DefferedExecution = true;
            string message = "";
            Given(() => message += "given");
            When(() => message += "when");
            message.Should().Be("given");
            WhenAction().Wait();
            message.Should().Be("givenwhen");
        }

        [Fact]
        public void Can_execute_async_method()
        {
            SetThe<IAsyncService>().To(new AsyncService());
            bool result = false;
            When(async () => result = await The<IAsyncService>().DoSomething());
            result.Should().Be(true);
        }

        interface IAsyncService
        {
            Task<bool> DoSomething();

        }

        private class AsyncService : IAsyncService
        {
            public async Task<bool> DoSomething()
            {
#if WINRT
                await Task.Delay(100);
#else
                await TaskEx.Delay(100);
#endif
                
                return true;
            }

        }
    }
}