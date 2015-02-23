namespace Chill.Http
{
    using System;
    using System.Threading.Tasks;

    public class WhenBuilder : ScenarioBuilder
    {
        public WhenBuilder(Scenario scenario) : base(scenario)
        {}

        public WhenBuilder Do(Func<Task> action)
        {
            Scenario.AddThens(() => new DebuggingAction(action));
            return new WhenBuilder(Scenario);
        }

        public WhenBuilder Then(Func<IUserAction> userAction)
        {
            Scenario.AddThens(userAction);
            return new WhenBuilder(Scenario);
        }
    }
}