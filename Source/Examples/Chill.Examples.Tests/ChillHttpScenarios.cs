namespace Chill.Examples.Tests
{
    using ExampleApp;
    using Http;
    using Xunit;

    public class ChillHttpScenarios : TestBase
    {

        public ChillHttpScenarios()
        {
            UseThe(new Scenario(new ExampleAppMiddleware().AppFunc));
        }

        [Fact]
        public void TestChill()
        {
            The<Scenario>()
                .WithUsers(All<Chill.Http.User>())
                .Execute();
        }

    }
}
