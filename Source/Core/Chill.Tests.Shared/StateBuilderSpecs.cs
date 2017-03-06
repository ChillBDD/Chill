using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Chill.AutofacFakeItEasy;
using Chill.AutofacNSubstitute;
using Chill.StateBuilders;
using Chill.Tests.TestSubjects;

using FluentAssertions;

using Xunit;

namespace Chill.Tests
{
    [ChillContainer(typeof(AutofacNSubstituteChillContainer))]

    public class StateBuilderSpecs : GivenWhenThen
    {
        AClass _expectedAClass = new AClass();

        [Fact]
        public void When_setting_items_directly_then_it_should_be_found()
        {
            When(() => UseThe(_expectedAClass));
        }

        [Fact]
        public void When_deferring_execution_then_action_is_not_executed_immediately()
        {
            When(() => { throw new Exception(); }, deferredExecution:true);
        }

        [Fact]
        public void When_deferring_execution_as_property_then_action_is_not_executed_immediately()
        {
            DeferredExecution = true;
            When(() => { throw new Exception(); });
        }
        
        [Fact]
        public void When_setting_multiple_items_then_they_should_be_set()
        {
            When(() => SetAll(new AClass("1"), new AClass("2")));

            All<AClass>().Should().BeEquivalentTo(new[] {new AClass("1"), new AClass("2"),});
        }

        [Fact]
        public void When_exception_is_thrown_in_async_method_in_deferred_execution_expected_exception_is_filled()
        {
            Func<Task> whenActionASync = () => Task.Factory.StartNew(() => { throw new AbandonedMutexException(); });
            When(whenActionASync, deferredExecution: true);
            CaughtException.Should().BeOfType<AbandonedMutexException>();
        }

        [Fact]
        public void When_exception_is_thrown_in_deferred_execution_expected_exception_is_filled()
        {
            Action whenActionASync = () => { throw new AbandonedMutexException(); };
            When(whenActionASync, deferredExecution:true);
            CaughtException.Should().BeOfType<AbandonedMutexException>();
        }

        [Fact]
        public void When_deferring_execution_then_whenaction_can_be_used_to_test_for_exceptions()
        {
            When(() => { throw new AbandonedMutexException(); }, deferredExecution: true);
            WhenAction.ShouldThrow<AbandonedMutexException>();
        }

        [Fact]
        public void When_no_exception_is_thrown_but_expected_exception_is_used_then_caughtexceptoin_throws()
        {
            When(() => { }, deferredExecution: true);
            Action a = () =>
            {
                Exception e = CaughtException;
            };
            a.ShouldThrow<InvalidOperationException>().WithMessage("Expected exception but no exception was thrown");
        }

        [Fact]
        public void When_setting_testclass_in_container_then_it_should_be_found()
        {
            When(() => SetThe<AClass>().To(_expectedAClass));

            The<AClass>().Should().Be(_expectedAClass);
        }

        [ChillContainer(typeof(AutofacFakeItEasyMockingContainer))]
        public class Given_several_testclasses_in_the_container : GivenWhenThen
        {
            public Given_several_testclasses_in_the_container()
            {
                Given(() =>
                {
                     SetThe<AClass>().Named("first").To(new AClass("first"));
                     SetThe<AClass>().Named("second").To(new AClass("second"));
                });
            }


            [Fact]
            public void then_named_testclass_is_present()
            {
                TheNamed<AClass>("first").Name.Should().Be("first");
            }

            [Fact]
            public void Then_all_should_be_contain_all_testclasses()
            {
                All<AClass>().Count().Should().Be(2);
            }
        }
    }
}