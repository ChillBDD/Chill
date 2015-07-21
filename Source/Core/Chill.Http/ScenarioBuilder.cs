namespace Chill.Http
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

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

        public Task Execute([CallerMemberName] string scenarioName = "")
        {
            return Scenario.Execute(scenarioName);
        }
    }
}