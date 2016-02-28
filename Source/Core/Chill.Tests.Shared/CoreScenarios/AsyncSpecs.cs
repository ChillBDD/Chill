using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Chill.Tests.CoreScenarios
{
    namespace AsyncSpecs
    {
        public class When_an_async_act_is_used : GivenWhenThen
        {
            private string result;

            public When_an_async_act_is_used()
            {
                When(async () =>
                {
                    await Task.Delay(1000.Milliseconds());
                    result = await Task.FromResult("hello from async WHEN");
                });
            }

            [Fact]
            public void Then_it_should_evaluate_the_sync_code_synchronously()
            {
                result.Should().Contain("hello");
            }
        }
    }
}
