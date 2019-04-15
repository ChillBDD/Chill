using System;
using System.Threading.Tasks;
using Chill.Specs.TestSubjects;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Chill.Specs
{
    public class GivenSubjectSpecs : GivenSubject<TestSubject>
    {
        [Fact]
        public void When_the_subject_has_dependencies_it_should_inject_them_from_the_container()
        {
            UseThe(Substitute.For<ITestService>());
            
            Subject.TestService.Should().NotBeNull();
        }

        [Fact]
        public void When_configuring_mock_in_given_then_injected_mock_should_be_same()
        {
            ITestService testService = null;

            Given(() =>
            {
                UseThe(Substitute.For<ITestService>());
                
                testService = The<ITestService>();
            });

            Subject.TestService.Should().BeSameAs(testService);
        }

        [Fact]
        public void When_using_both_givens_and_whens_the_givens_are_executed_before_the_when()
        {
            string message = "";
            
            Given(() => message += "given1");
            
            Given(() => message += "given2");
            
            When(() => message += "when");
            
            message.Should().Be("given1given2when");
        }

        [Fact]
        public void When_calling_the_when_in_deferred_mode_it_should_not_execute_it_until_the_action_is_evaluated()
        {
            string message = "";
            Given(() => message += "given");
            
            When(() => message += "when", deferredExecution: true);
            
            message.Should().Be("given");
            
            WhenAction();
            
            message.Should().Be("givenwhen");
        }

        [Fact]
        public void When_configuring_the_test_to_use_deferred_mode_the_when_should_not_execute_until_it_is_requested()
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
        public void When_setting_up_the_call_to_when_later_then_whenaction_is_not_called_automatically()
        {
            string message = "";
            
            Given(() => message += "given");
            
            WhenLater(() => message += "when");
            
            message.Should().Be("given");
            
            WhenAction();
            
            message.Should().Be("givenwhen");
        }

        [Fact]
        public void When_the_when_is_async_it_should_await_the_body()
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
                await Task.Delay(100);

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
    
    public class TestSubject
    {
        private readonly ITestService testService;

        public TestSubject(ITestService testService)
        {
            this.testService = testService;
        }

        public bool DoSomething()
        {
            return testService.TryMe();
        }

        public ITestService TestService
        {
            get { return testService; }
        }
    }

}