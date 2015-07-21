namespace Chill.Http
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public class ScenarioBuilder
    {

        public ScenarioBuilder(HttpTestScenario scenario, string defaultCommandsPath, string defaultQueryPath)
        {
            Scenario = scenario;
        }

        protected ScenarioBuilder(HttpTestScenario scenario)
        {
            Scenario = scenario;
        }

        protected HttpTestScenario Scenario { get; private set; }
        public Task Execute(string scenarioName = null)
        {
            if (scenarioName == null)
            {
                scenarioName = BuidScenarioNameFromCallingClassAndMethodName();
            }

            return Scenario.Execute(scenarioName.Humanize());
        }

        private static string BuidScenarioNameFromCallingClassAndMethodName()
        {
            string scenarioName;
            // Note, if you get the scenarioname : Runtimemethodhandle invokemethod, then it means the test assembly is being optimized. 
            // Turn off optimziation for release builds (only for the test assembly) and it works again. 
            var stackFrame = new StackFrame(2, false);
            var callingMethod = stackFrame.GetMethod();
            var declaringType = callingMethod.DeclaringType;
            var parentClassName = declaringType.Name;

            scenarioName = parentClassName + "_" + callingMethod.Name;
            return scenarioName;
        }

        /// <summary>
        /// Implicitly converting the scenariobuilder to a task executes it;
        /// </summary>
        /// <param name="builder"></param>
        public static implicit operator Task(ScenarioBuilder builder)
        {
            
            return builder.Execute(BuidScenarioNameFromCallingClassAndMethodName());
        }

    }

}