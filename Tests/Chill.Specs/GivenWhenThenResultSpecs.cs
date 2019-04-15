using FluentAssertions;
using Xunit;

namespace Chill.Specs
{
    public class GivenWhenThenResultSpecs : GivenWhenThen<string>
    {
        [Fact]
        public void When_a_deferred_executed_when_later_is_invoked_it_should_return_the_result()
        {
            WhenLater(() => "hello");

            WhenAction();

            Result.Should().Be("hello");
        }
    }
}