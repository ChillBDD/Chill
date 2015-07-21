

namespace Chill.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Chill.Http.Logging;
    using Chill.Http.Logging.LogProviders;

    public class HttpTestScenario
    {
        private static readonly ILog s_log = LogProvider.GetCurrentClassLogger();
        private readonly HttpClientBuilder _clientBuilder;
        private readonly Action<string> _consoleWriter;
        private readonly List<Func<IUserAction>> _givens = new List<Func<IUserAction>>();
        private readonly List<Func<IUserAction>> _thens = new List<Func<IUserAction>>();
        private readonly List<TestUser> _users = new List<TestUser>();
        private bool _errorOccurred;

        public HttpTestScenario(HttpClientBuilder clientBuilder, Action<string> consoleWriter = null)
        {
            _clientBuilder = clientBuilder;
            _consoleWriter = consoleWriter ?? Console.WriteLine;
        }

        internal Func<IUserAction> When { get; set; }

        public GivenBuilder WithUsers(IEnumerable<TestUser> users)
        {
            _users.AddRange(users);
            return new GivenBuilder(this);
        }

        public GivenBuilder WithUsers(params TestUser[] testUsers)
        {
            return WithUsers((IEnumerable<TestUser>) testUsers);
        }

        internal void AddGivens(params Func<IUserAction>[] givens)
        {
            _givens.AddRange(givens);
        }

        internal void AddThens(params Func<IUserAction>[] assertions)
        {
            _thens.AddRange(assertions);
        }

        private List<string> _messages = new List<string>(); 

        public void AddLogMessage(string message)
        {
            _messages.Add(message);
        }

        public async Task Execute(string scenarioName)
        {
            using(var listener = SetupTraceListener())
            {
                var scenarioResult = new ScenarioResult(scenarioName);

                foreach(var user in _users)
                {
                    //TODO Erwin this should async, OR authentication is a seperate step to BuildClient()
                    user.Initialize(_clientBuilder);
                }
                var index = 1;
                _errorOccurred = false;
                foreach(var given in _givens)
                {
                    s_log.InfoFormat("*** Executing Given {0}", index);
                    scenarioResult.Givens.Add(await ExecuteTestStep(given, "Given", index++));
                }
                if(When != null)
                {
                    s_log.InfoFormat("*** Executing When {0}", index);
                    scenarioResult.When = await ExecuteTestStep(When, "When", index++);
                }
                foreach(var then in _thens)
                {
                    s_log.InfoFormat("*** Executing Then {0}", index);
                    scenarioResult.Thens.Add(await ExecuteTestStep(then, "Then", index++));
                }

                string failureMessage = null;
                var firstFailedStep = scenarioResult.FirstException();
                if(firstFailedStep != null)
                {
                    failureMessage = "** This test failed at step: " + firstFailedStep.StepType + " " +
                                     firstFailedStep.StepIndex + ": ";

                    _consoleWriter("**FAILURE *************************************");
                    _consoleWriter(failureMessage + firstFailedStep.Message + Environment.NewLine + Environment.NewLine +
                                   "\t Cause: " +
                                   firstFailedStep.GetExceptionMessage() + Environment.NewLine);
                }

                _consoleWriter("***********************************************");
                _consoleWriter("Scenario: " + scenarioName);
                _consoleWriter("***********************************************");

                _consoleWriter(scenarioResult.ToString());
                _consoleWriter("***********************************************");
                _consoleWriter("Log messages:");
                _consoleWriter("***********************************************");

                foreach(var message in _messages)
                {
                    _consoleWriter(message + "\r\n");
                }

                _consoleWriter(string.Join("\r\n", listener.LogMessages.ToList()));

                if(firstFailedStep != null)
                {
                    throw new TestFailedException(failureMessage);
                }
            }
        }


        protected virtual InMemoryTraceListener SetupTraceListener()
        {
            var inMemoryTraceListener = new InMemoryTraceListener();
            Trace.Listeners.Clear();
            Trace.Listeners.Add(inMemoryTraceListener);

            return inMemoryTraceListener;
        }

        private async Task<StepResult> ExecuteTestStep(Func<IUserAction> step, string stepType, int index)
        {
            IUserAction userAction;
            var stepResult = new StepResult(index, stepType);
            try
            {
                userAction = step();
            }
            catch(Exception ex)
            {
                stepResult.Exception = ex;
                _errorOccurred = true;
                return stepResult;
            }

            stepResult.Message = userAction.Message;
            if(!_errorOccurred)
            {
                try
                {
                    await userAction.Execute();
                }
                catch(Exception ex)
                {
                    _errorOccurred = true;
                    stepResult.Exception = ex;
                }
            }
            var actionindex = 1;
            foreach(var action in userAction.ResultActions.NullToEmpty().ToList())
            {
                var stepAssertion = new StepAssertion(actionindex++, action.Message);
                stepResult.StepAssertions.Add(stepAssertion);
                try
                {
                    if(!_errorOccurred)
                    {
                        action.Assert();
                    }
                }
                catch(Exception ex)
                {
                    _errorOccurred = true;
                    stepAssertion.Exception = ex;
                }
            }

            return stepResult;
        }
    }

    /// <summary>
    /// The testfailed exception is designed to reduce clutter in the log messages section of the tests. 
    /// By not showing the stacktrace, it tries to force you to focuss on where the tests really fail. 
    /// </summary>
    internal class TestFailedException : Exception
    {
        public TestFailedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Hide the stacktrace in the output, since that doesn't make any sense to display in a chill test
        /// </summary>
        public override string StackTrace
        {
            get { return ""; }
        }

        public override string ToString()
        {
            return Message;
        }
    }

    public class ScenarioResult
    {
        private static readonly ILog s_log = LogProvider.GetCurrentClassLogger();

        private readonly string _scenarioName;

        public ScenarioResult(string scenarioName)
        {
            _scenarioName = scenarioName;
            Givens = new List<StepResult>();
            Thens = new List<StepResult>();
        }

        public List<StepResult> Givens { get; }
        public StepResult When { get; set; }
        public List<StepResult> Thens { get; }

        public StepResult FirstException()
        {
            return Givens.FirstOrDefault(x => x.HasException())
                   ?? WhenIfException()
                   ?? Thens.FirstOrDefault(x => x.HasException());
        }

        public StepResult WhenIfException()
        {
            if(When != null && When.HasException())
            {
                return When;
            }
            return null;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            if(Givens.Any())
            {
                builder.AppendLine("Given:");
                foreach(var given in Givens)
                {
                    builder.AppendLine(given.ToString());
                }
            }

            if(When != null)
            {
                builder.AppendLine("When:");
                builder.AppendLine(When.ToString());
            }

            if(Thens.Any())
            {
                builder.AppendLine("Then:");
                foreach(var then in Thens)
                {
                    builder.AppendLine(then.ToString());
                }
            }

            return builder.ToString();
        }
    }

    public class StepAssertion
    {
        public StepAssertion(int index, string message)
        {
            Index = index;
            Message = message;
        }

        public int Index { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }

    public class StepResult
    {
        public StepResult(int stepIndex, string stepType)
        {
            StepIndex = stepIndex;
            StepType = stepType;
            StepAssertions = new List<StepAssertion>();
        }

        public int StepIndex { get; }
        public string StepType { get; }
        public List<StepAssertion> StepAssertions { get; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public bool HasException()
        {
            return Exception != null || StepAssertions.Any(x => x.Exception != null);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            if(HasException())
            {
                builder.Append("....*** Failure in: " + StepType + " " + StepIndex + " - " + FormatMessage(Message) +
                               Environment.NewLine);
            }
            else if(Message != null)
            {
                builder.Append("...." + StepType + " " + StepIndex + " - " + FormatMessage(Message) +
                               Environment.NewLine);
            }

            if(StepAssertions.Any())
            {
                builder.Append("....Assertions:" + Environment.NewLine);
                foreach(var assertion in StepAssertions)
                {
                    if(assertion.Exception != null)
                    {
                        builder.Append("........***Failed: " + StepType + " " + StepIndex + "." + assertion.Index +
                                       " - " + FormatAssertionMessage(assertion) +
                                       Environment.NewLine);
                    }
                    else
                    {
                        builder.Append("........" + StepType + " " + StepIndex + "." + assertion.Index + " - " +
                                       assertion.Message + Environment.NewLine);
                    }
                }
            }

            if(Exception != null)
            {
                builder.Append("....See exception details below" + Environment.NewLine);
            }
            return builder.ToString();
        }

        private static string FormatAssertionMessage(StepAssertion assertion)
        {
            if(assertion == null)
            {
                return "{null}";
            }
            return FormatMessage(assertion.Message);
        }

        private static string FormatMessage(string message)
        {
            return (message ?? "{null}").Replace(Environment.NewLine, Environment.NewLine + "....");
        }

        public string GetExceptionMessage()
        {
            var exception = Exception;
            if(exception == null)
            {
                var failed = StepAssertions.FirstOrDefault(x => x.Exception != null);
                if(failed != null)
                {
                    exception = failed.Exception;
                }
            }
            if(exception == null)
            {
                return null;
            }
            return exception.ToString();
        }
    }

    internal class SystemDiagnosticsLogProvider : LogProviderBase, ILogProvider
    {
        Logger ILogProvider.GetLogger(string name)
        {
            Logger logger =
                (LogLevel logLevel, Func<string> messageFunc, Exception exception, object[] formatParameters) =>
                {
                    if(messageFunc == null)
                    {
                        return true;
                    }
                    var logMessage = LogMessageFormatter.SimulateStructuredLogging(messageFunc, formatParameters)();

                    string exceptionMessage = null;
                    if(exception != null)
                    {
                        exceptionMessage = " - " + exception;
                    }

                    Trace.WriteLine(logLevel + " - " + logMessage + exceptionMessage);

                    return true;
                };
            return logger;
        }

        public static void SetupLogProvider()
        {
            LogProvider.SetCurrentLogProvider(new SystemDiagnosticsLogProvider());
            LogProvider.SetCurrentLogProvider(new SystemDiagnosticsLogProvider());
        }

        public override Logger GetLogger(string name)
        {
            Logger logger =
                (LogLevel logLevel,
                    Func<string> messageFunc,
                    Exception exception,
                    object[] formatParameters) =>
                {
                    if(messageFunc == null)
                    {
                        return true;
                    }
                    var logMessage = LogMessageFormatter.SimulateStructuredLogging(messageFunc, formatParameters)();

                    string exceptionMessage = null;
                    if(exception != null)
                    {
                        exceptionMessage = " - " + exception;
                    }

                    Trace.WriteLine(logLevel + " - " + logMessage + exceptionMessage);

                    return true;
                };
            return logger;
        }
    }


    public class InMemoryTraceListener : TraceListener
    {
        private readonly StringBuilder _buffer = new StringBuilder();
        private readonly List<string> _messages = new List<string>();
        private readonly Stopwatch stopwatch;

        public InMemoryTraceListener()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public IEnumerable<string> LogMessages
        {
            get
            {
                FlushBuffer();
                return _messages;
            }
        }

        public override void Write(string message)
        {
            var currentExecutableFileName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            if(message.StartsWith(currentExecutableFileName))
            {
                message = message.Substring(currentExecutableFileName.Length).TrimStart();
            }

            _buffer.Append("\t" + stopwatch.ElapsedMilliseconds + "ms - " + message);
        }

        public override void WriteLine(string message)
        {
            _buffer.Append("\t" + stopwatch.ElapsedMilliseconds + "ms - " + message);
            FlushBuffer();
        }

        private void FlushBuffer()
        {
            if(_buffer.Capacity > 0)
            {
                _messages.Add(_buffer.ToString());
                _buffer.Clear();
            }
        }

        protected override void Dispose(bool disposing)
        {
            Trace.Listeners.Remove(this);
            base.Dispose(disposing);
        }
    }

    public class AssertionResult
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}