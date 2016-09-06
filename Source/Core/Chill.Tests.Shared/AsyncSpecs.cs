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