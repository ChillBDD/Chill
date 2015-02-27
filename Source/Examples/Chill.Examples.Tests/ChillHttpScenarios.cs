namespace Chill.Examples.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using ExampleApp;
    using Http;
    using Xunit;

    // Collect log messages and print at bottom
    // Inject HttpClient
    // 


    public class ChillHttpScenarios : TestBase
    {

        public ChillHttpScenarios()
        {
            UseThe(new Scenario(new ExampleAppMiddleware().AppFunc));
            UseThe(new User("erwin", "password"));
        }


        [Fact]
        public async Task TestChill2()
        {
            await The<Scenario>()
                .WithUsers(All<User>())
                .Given(() => The<User>()
                    .Gets("/test"))
                .Execute();
        }
    }
}
