using System;
using System.Threading.Tasks;
using Chill.Tests.TestSubjects;

using FluentAssertions;
using Xunit;

namespace Chill.Tests.CoreScenarios
{
    public abstract class GivenSubjectSpecs : GivenSubject<TestSubject>
    {
        [Fact]
        public void When_creating_a_subject_then_gets_dependencies_injected()
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
        public void When_getting_mock_it_should_be_created()
        {
            The<ITestService>().Should().NotBeNull();
        }

        [Fact]
        public void When_getting_named_service_it_should_be_created()
        {
            TheNamed<ITestService>("abc").Should().NotBeNull();
        }

        [Fact]
        public void When_using_both_givens_and_whens_the_givens_are_executed_before_the_when()
        {
            string message = "";
            Given(() => message += "given");
            Given(() => message += "given");
            When(() => message += "when");
            message.Should().Be("givengivenwhen");
        }

        [Fact]
        public void When_calling_when_deferred_then_whenaction_is_not_called_automatically()
        {
            string message = "";
            Given(() => message += "given");
            When(() => message += "when", deferredExecution: true);
            message.Should().Be("given");
            WhenAction();
            message.Should().Be("givenwhen");
        }

        [Fact]
        public void When_deferred_and_calling_when_then_whenaction_is_not_called_automatically()
        {
            DeferredExecution = true;
            string message = "";
            Given(() => message += "given");
            When(() => message += "when");
            message.Should().Be("given");
            WhenAction();
            message.Should().Be("givenwhen");
        }

        [Fact]
        public void When_calling_whenlater_then_whenaction_is_not_called_automatically()
        {
            string message = "";
            Given(() => message += "given");
            WhenLater(() => message += "when");
            message.Should().Be("given");
            WhenAction();
            message.Should().Be("givenwhen");
        }

        [Fact]
        public void When_calling_async_when_method_the_func_is_awaited()
        {
            SetThe<IAsyncService>().To(new AsyncService());
            bool result = false;
            When(async () => result = await The<IAsyncService>().DoSomething());
            result.Should().Be(true);
        }

        [Fact]
        public void When_calling_async_given_the_func_is_awaited()
        {
            SetThe<IAsyncService>().To(new AsyncService());
            bool result = false;
            Given(async () => result = await The<IAsyncService>().DoSomething());
            result.Should().Be(true);
        }

        private interface IAsyncService
        {
            Task<bool> DoSomething();
        }

        private class AsyncService : IAsyncService
        {
            public async Task<bool> DoSomething()
            {
#if NET45
                await Task.Delay(100);
#else
                await TaskEx.Delay(100);
#endif

                return true;
            }
        }
    }

    public class GivenSubjectResultSpecs : GivenSubject<object, string>
    {
        [Fact]
        public void When_calling_when_deferred_then_whenaction_is_called_on_result()
        {
            string message = "";
            Given(() => message += "given");
            When(() => message += "when", deferredExecution: true);
            message.Should().Be("given");
            Result.Should().Be("givenwhen");
        }

        [Fact]
        public void When_calling_when_directly_then_whenaction_is_not_called_on_result()
        {
            string message = "";
            Given(() => message += "given");
            When(() => message += "when");
            message.Should().Be("givenwhen");
            Result.Should().Be("givenwhen");
        }

        [Fact]
        public void When_calling_when_later_then_whenaction_is_called_on_result()
        {
            string message = "";
            Given(() => message += "given");
            WhenLater(() => message += "when");
            message.Should().Be("given");
            Result.Should().Be("givenwhen");
        }
    }

    public class When_a_subject_is_build_using_a_custom_factory : GivenSubject<Disposable>
    {
        private readonly Disposable subject = new Disposable();

        public When_a_subject_is_build_using_a_custom_factory()
        {
            Given(() =>
            {
                WithSubject(_ => subject);
            });

            When(() =>
            {
                // NOTE: Force the subject to get created
                Subject.Foo();
            });
        }

        [Fact]
        public void Then_dispose_should_still_dispose_our_subject()
        {
            // NOTE: Because Dispose is called later, we just need this method to mark it as a test-case.
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            subject.IsDisposed.Should().BeTrue("because even custom-built subjects should be disposed");
        }
    }

    public class Disposable : IDisposable
    {
        public void Dispose()
        {
            IsDisposed = true;
        }

        public void Foo()
        {
        }

        public bool IsDisposed { get; set; }
    }
}