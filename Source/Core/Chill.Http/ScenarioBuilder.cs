namespace Chill.Http
{
    using System.Runtime.CompilerServices;

    public class ScenarioBuilder
    {

        public ScenarioBuilder(Scenario scenario, string defaultCommandsPath, string defaultQueryPath)
        {
            Scenario = scenario;
        }

        protected ScenarioBuilder(Scenario scenario)
        {
            Scenario = scenario;
        }

        protected Scenario Scenario { get; private set; }

        public void Execute([CallerMemberName] string scenarioName = "")
        {
            Scenario.Execute(scenarioName);
        }
    }
}