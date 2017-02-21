using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Chill.Tests.Shared
{
    namespace AsyncSpecs
    {
        public class When_an_async_act_is_used : GivenWhenThen
        {
            private BlockingCollection<int> results = new BlockingCollection<int>();

            public When_an_async_act_is_used()
            {
                When(async () =>
                {
                    foreach (int key in Enumerable.Range(0, 1000))
                    {
#if NET45
                        await Task.Delay(10);
#else
                        await TaskEx.Delay(10);                            
#endif
                        results.Add(key);
                    }
                });
            }

            [Fact]
            public void Then_it_should_evaluate_the_sync_code_synchronously()
            {
                results.Should().HaveCount(1000);
            }
        }

        public class When_a_deferred_async_act_throws : GivenWhenThen
        {
            public When_a_deferred_async_act_throws()
            {
                When(
                    async () =>
                    {
#if NET45
                        await Task.Delay(10);
#else
                        await TaskEx.Delay(10);
#endif
                        throw new ApplicationException();
                    },
                    deferredExecution: true);
            }

            [Fact]
            public void Then_the_exception_should_be_observed()
            {
                WhenAction.ShouldThrow<ApplicationException>();
            }

            [Fact]
            public void Then_the_exception_should_not_be_wrapped_to_an_aggregate_exception()
            {
                WhenAction.ShouldNotThrow<AggregateException>();
            }
        }

        public class When_a_deferred_async_act_throws_in_a_test_with_result : GivenWhenThen<object>
        {
            public When_a_deferred_async_act_throws_in_a_test_with_result()
            {
                When((Func<Task<object>>)(
                    async () =>
                    {
#if NET45
                        await Task.Delay(10);
#else
                        await TaskEx.Delay(10);
#endif
                        throw new ApplicationException();
                    }),
                    deferredExecution: true);
            }

            [Fact]
            public void Then_the_exception_should_be_observed()
            {
                Action action = () => WhenAction();
                action.ShouldThrow<ApplicationException>();
            }

            [Fact]
            public void Then_the_exception_should_not_be_wrapped_to_an_aggregate_exception()
            {
                Action action = () => WhenAction();
                action.ShouldNotThrow<AggregateException>();
            }
        }

        public class When_a_deferred_async_act_throws_in_a_test_with_subject : GivenSubject<object>
        {
            public When_a_deferred_async_act_throws_in_a_test_with_subject()
            {
                When(
                    async () =>
                    {
#if NET45
                        await Task.Delay(10);
#else
                        await TaskEx.Delay(10);
#endif
                        throw new ApplicationException();
                    },
                    deferredExecution: true);
            }

            [Fact]
            public void Then_the_exception_should_be_observed()
            {
                WhenAction.ShouldThrow<ApplicationException>();
            }

            [Fact]
            public void Then_the_exception_should_not_be_wrapped_to_an_aggregate_exception()
            {
                WhenAction.ShouldNotThrow<AggregateException>();
            }
        }

        public class When_a_deferred_async_act_throws_in_a_test_with_subject_and_result : GivenSubject<object, object>
        {
            public When_a_deferred_async_act_throws_in_a_test_with_subject_and_result()
            {
                When((Func<Task<object>>)(
                    async () =>
                    {
#if NET45
                        await Task.Delay(10);
#else
                        await TaskEx.Delay(10);
#endif
                        throw new ApplicationException();
                    }),
                    deferredExecution: true);
            }

            [Fact]
            public void Then_the_exception_should_be_observed()
            {
                Action action = () => WhenAction();
                action.ShouldThrow<ApplicationException>();
            }

            [Fact]
            public void Then_the_exception_should_not_be_wrapped_to_an_aggregate_exception()
            {
                Action action = () => WhenAction();
                action.ShouldNotThrow<AggregateException>();
            }
        }

        public class When_an_async_arrange_is_used : GivenWhenThen
        {
            private BlockingCollection<int> results = new BlockingCollection<int>();

            public When_an_async_arrange_is_used()
            {
                Given(async () =>
                {
                    foreach (int key in Enumerable.Range(0, 1000))
                    {
#if NET45
                        await Task.Delay(10);
#else
                        await TaskEx.Delay(10);                            
#endif
                        results.Add(key);
                    }
                });
            }

            [Fact]
            public void Then_it_should_evaluate_the_sync_code_synchronously()
            {
                results.Should().HaveCount(1000);
            }
        }
    }
}