using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Chill.Specs
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
                        await Task.Delay(10);
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

        public class When_later_async_act_throws : GivenWhenThen
        {
            public When_later_async_act_throws()
            {
                WhenLater(
                    async () =>
                    {
                        await Task.Delay(10);
                        throw new ApplicationException();
                    });
            }

            [Fact]
            public void Then_the_exception_should_be_observed()
            {
                WhenAction.Should().Throw<ApplicationException>();
            }

            [Fact]
            public void Then_the_exception_should_not_be_wrapped_to_an_aggregate_exception()
            {
                WhenAction.Should().NotThrow<AggregateException>();
            }
        }

        public class When_a_deferred_async_act_throws : GivenWhenThen
        {
            public When_a_deferred_async_act_throws()
            {
                When(
                    async () =>
                    {
                        await Task.Delay(10);
                        throw new ApplicationException();
                    },
                    deferredExecution: true);
            }

            [Fact]
            public void Then_the_exception_should_be_observed()
            {
                WhenAction.Should().Throw<ApplicationException>();
            }

            [Fact]
            public void Then_the_exception_should_not_be_wrapped_to_an_aggregate_exception()
            {
                WhenAction.Should().NotThrow<AggregateException>();
            }
        }

        public class When_later_async_act_throws_in_a_test_with_result : GivenWhenThen<object>
        {
            public When_later_async_act_throws_in_a_test_with_result()
            {
                WhenLater((Func<Task<object>>) (
                    async () =>
                    {
                        await Task.Delay(10);
                        throw new ApplicationException();
                    }));
            }

            [Fact]
            public void Then_the_exception_should_be_observed()
            {
                Action action = () => WhenAction();
                action.Should().Throw<ApplicationException>();
            }

            [Fact]
            public void Then_the_exception_should_not_be_wrapped_to_an_aggregate_exception()
            {
                Action action = () => WhenAction();
                action.Should().NotThrow<AggregateException>();
            }
        }

        public class When_a_deferred_async_act_throws_in_a_test_with_result : GivenWhenThen<object>
        {
            public When_a_deferred_async_act_throws_in_a_test_with_result()
            {
                When((Func<Task<object>>)(
                    async () =>
                    {
                        await Task.Delay(10);
                        throw new ApplicationException();
                    }),
                    deferredExecution: true);
            }

            [Fact]
            public void Then_the_exception_should_be_observed()
            {
                Action action = () => WhenAction();
                action.Should().Throw<ApplicationException>();
            }

            [Fact]
            public void Then_the_exception_should_not_be_wrapped_to_an_aggregate_exception()
            {
                Action action = () => WhenAction();
                action.Should().NotThrow<AggregateException>();
            }
        }

        public class When_a_deferred_async_act_throws_in_a_test_with_subject : GivenSubject<object>
        {
            public When_a_deferred_async_act_throws_in_a_test_with_subject()
            {
                When(
                    async () =>
                    {
                        await Task.Delay(10);
                        throw new ApplicationException();
                    },
                    deferredExecution: true);
            }

            [Fact]
            public void Then_the_exception_should_be_observed()
            {
                WhenAction.Should().Throw<ApplicationException>();
            }

            [Fact]
            public void Then_the_exception_should_not_be_wrapped_to_an_aggregate_exception()
            {
                WhenAction.Should().NotThrow<AggregateException>();
            }
        }

        public class When_later_async_act_throws_in_a_test_with_subject : GivenSubject<object>
        {
            public When_later_async_act_throws_in_a_test_with_subject()
            {
                WhenLater(
                    async () =>
                    {
                        await Task.Delay(10);
                        throw new ApplicationException();
                    });
            }

            [Fact]
            public void Then_the_exception_should_be_observed()
            {
                WhenAction.Should().Throw<ApplicationException>();
            }

            [Fact]
            public void Then_the_exception_should_not_be_wrapped_to_an_aggregate_exception()
            {
                WhenAction.Should().NotThrow<AggregateException>();
            }
        }

        public class When_a_deferred_async_act_throws_in_a_test_with_subject_and_result : GivenSubject<object, object>
        {
            public When_a_deferred_async_act_throws_in_a_test_with_subject_and_result()
            {
                When((Func<Task<object>>)(
                    async () =>
                    {
                        await Task.Delay(10);
                        throw new ApplicationException();
                    }),
                    deferredExecution: true);
            }

            [Fact]
            public void Then_the_exception_should_be_observed()
            {
                Action action = () => WhenAction();
                action.Should().Throw<ApplicationException>();
            }

            [Fact]
            public void Then_the_exception_should_not_be_wrapped_to_an_aggregate_exception()
            {
                Action action = () => WhenAction();
                action.Should().NotThrow<AggregateException>();
            }
        }

        public class When_later_async_act_throws_in_a_test_with_subject_and_result : GivenSubject<object, object>
        {
            public When_later_async_act_throws_in_a_test_with_subject_and_result()
            {
                WhenLater((Func<Task<object>>)(
                    async () =>
                    {
                        await Task.Delay(10);
                        throw new ApplicationException();
                    }));
            }

            [Fact]
            public void Then_the_exception_should_be_observed()
            {
                Action action = () => WhenAction();
                action.Should().Throw<ApplicationException>();
            }

            [Fact]
            public void Then_the_exception_should_not_be_wrapped_to_an_aggregate_exception()
            {
                Action action = () => WhenAction();
                action.Should().NotThrow<AggregateException>();
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
                        await Task.Delay(10);
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