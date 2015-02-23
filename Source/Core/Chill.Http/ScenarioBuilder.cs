namespace Chill.Http
{
    using System.Runtime.CompilerServices;

    public class ScenarioBuilder
    {

        public ScenarioBuilder(Scenario scenario, string defaultCommandsPath, string defaultQueryPath)
        {
            scenario.DefaultCommandsPath = defaultCommandsPath;
            scenario.DefaultQueryPath = defaultQueryPath;
            Scenario = scenario;
        }

        protected ScenarioBuilder(Scenario scenario)
        {
            Scenario = scenario;
        }

        protected Scenario Scenario { get; private set; }

        public void Execute(string commandPath = null, string queryPath = null, [CallerMemberName] string scenarioName = "")
        {
            Scenario.Execute(scenarioName, commandPath ?? Scenario.DefaultCommandsPath, queryPath ?? Scenario.DefaultQueryPath);
        }
    }
}