namespace Chill.Http
{
    using System;
    using System.Threading.Tasks;

    public class GivenBuilder : ScenarioBuilder
    {
        public GivenBuilder(Scenario scenario) : base(scenario)
        {}

        public GivenBuilder Given(params Func<IUserAction>[] givens)
        {
            Scenario.AddGivens(givens);
            return new GivenBuilder(Scenario);
        }

        public GivenBuilder<TResult> Given<TResult>(params Func<IUserAction<TResult>>[] givens)
        {
            foreach (var given in givens)
            {
                Scenario.AddGivens(given);
            }
            return new GivenBuilder<TResult>(Scenario);
        }

        public GivenBuilder Debug(Func<Task> action)
        {
            Scenario.AddGivens(() => new DebuggingAction(action));
            return new GivenBuilder(Scenario);
        }

        public WhenBuilder When(Func<IUserAction> doFunc)
        {
            Scenario.When = doFunc;
            return new WhenBuilder(Scenario);
        }

        public WhenBuilder Then(params Func<IUserAction>[] assertions)
        {
            Scenario.AddThens(assertions);
            return new WhenBuilder(Scenario);
        }
    }

    public class GivenBuilder<TResult> : GivenBuilder
    {
        public GivenBuilder(Scenario scenario) : base(scenario)
        {}
    }
}