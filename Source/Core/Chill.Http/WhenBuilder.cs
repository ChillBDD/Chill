namespace Chill.Http
{
    using System;
    using System.Threading.Tasks;

    public class WhenBuilder : ScenarioBuilder
    {
        public WhenBuilder(HttpTestScenario scenario) : base(scenario)
        {
        }

        public WhenBuilder Do(Func<Task> action)
        {
            Scenario.AddThens(() => new DebuggingAction(action));
            return new WhenBuilder(Scenario);
        }

        public WhenBuilder Then(params Func<IUserAction>[] assertions)
        {
            Scenario.AddThens(assertions);
            return new WhenBuilder(Scenario);
        }
    }
}