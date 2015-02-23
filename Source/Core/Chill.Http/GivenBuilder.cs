namespace Chill.Http
{
    using System;

    public class GivenBuilder : ScenarioBuilder
    {
        public GivenBuilder(Scenario scenario) : base(scenario)
        {}

        public GivenBuilder Given(Func<IUserAction> doFunc)
        {
            Scenario.AddGiven(doFunc);
            return new GivenBuilder(Scenario);
        }

        public GivenBuilder<TResult> Given<TResult>(Func<IUserAction<TResult>> doFunc)
        {
            Scenario.AddGiven(doFunc);
            return new GivenBuilder<TResult>(Scenario);
        }

        public GivenBuilder Debug(Action action)
        {
            Scenario.AddGiven(() => new DebuggingAction(action));
            return new GivenBuilder(Scenario);
        }

        public WhenBuilder When(Func<IUserAction> doFunc)
        {
            Scenario.When = doFunc;
            return new WhenBuilder(Scenario);
        }

        public WhenBuilder Then(Func<IUserAction> userAction)
        {
            Scenario.AddThens(userAction);
            return new WhenBuilder(Scenario);
        }
    }

    public class GivenBuilder<TResult> : GivenBuilder
    {
        public GivenBuilder(Scenario scenario) : base(scenario)
        {}
    }
}