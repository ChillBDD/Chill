namespace Chill.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Owin;
    using Microsoft.Owin.Builder;
    using Owin;

    using AppFunc = System.Func<
        System.Collections.Generic.IDictionary<string, object>,
        System.Threading.Tasks.Task
    >;

    using MidFunc = System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>,
        System.Threading.Tasks.Task
        >, System.Func<System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>
        >;



    public class HttpClientBuilder
    {
        private readonly Func<User, Task> _onAuthenticate;
        private readonly AppFunc _appFunc;
        private readonly string _baseAddress;
        private readonly string _url;



        public HttpClientBuilder(string baseAddress, Func<User, Task> onAuthenticate, AppFunc appFunc = null)
        {
            _onAuthenticate = onAuthenticate;
            _appFunc = appFunc;
            _baseAddress = baseAddress;
        }

        public virtual HttpClient Build(User user)
        {
            HttpMessageHandler handler;

            if (_appFunc == null)
            {
                handler = new HttpClientHandler()
                {
                    AllowAutoRedirect = true,
                    UseCookies = true,
                    CookieContainer = new CookieContainer()
                };
            }
            else
            {
                handler = new OwinHttpMessageHandler(_appFunc)
                {
                    AllowAutoRedirect = true,
                    UseCookies = true,
                    CookieContainer = new CookieContainer()
                };
            }

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(_baseAddress, UriKind.Absolute)
            };

        }

        public Task Authenticate(User user)
        {
            return _onAuthenticate(user);
        }
    }

    public class SimulatedAuthHttpClientBuilder : HttpClientBuilder
    {
        private readonly List<User> _authenticatedUsers = new List<User>(); 

        public SimulatedAuthHttpClientBuilder(string baseAddress, AppFunc appFunc, Func<User, Task> onAuthenticate = null)
            : base(baseAddress, onAuthenticate ?? SimulateAuthentication, BuildSimulateAppFunc(appFunc))
        {
            
        }

        private static AppFunc BuildSimulateAppFunc(AppFunc appFunc)
        {
            var app = new AppBuilder();
            app.Use(SimulateMidFunc());
            app.Run(ctx => appFunc(ctx.Environment));
            return app.Build();
        }

        public static MidFunc SimulateMidFunc()
        {
            return next => ctx =>
            {
                var owinContext = new OwinContext(ctx);

                if (owinContext.Request.Headers.Any(x => x.Key == "simulatedAuthenticationUserName"))
                {
                    var identity = new ClaimsIdentity(new[] { new Claim("name", owinContext.Request.Headers.Get("simulatedAuthenticationUserName")) }, "SimulatedAuthentication");
                    var principal = new ClaimsPrincipal(identity);

                    owinContext.Request.User = principal;
                    
                }
                
                return next(ctx);
            };
        }

        private static Task SimulateAuthentication(User user)
        {
            user.Client.DefaultRequestHeaders.Add("simulatedAuthenticationUserName", user.Name);
            return Task.FromResult(0);
        }

        public override HttpClient Build(User user)
        {
            var httpClient = base.Build(user);
            return httpClient;
        }
    }

        /* 
     * Scenario(new url);
     * 
     * Scenario(appFunc, simulateAuth=true)
     * 
     * new User(name);
     * 
     * new User(name);
     * 
     * User.LogIn()
     * */


    public class Scenario
    {
        private readonly HttpClientBuilder _clientBuilder;
        private readonly string _baseUrl;
        private readonly AppFunc _appFunc;
        private readonly List<Func<IUserAction>> _givens = new List<Func<IUserAction>>();
        private readonly List<Func<IUserAction>> _thens = new List<Func<IUserAction>>();
        private readonly List<User> _users = new List<User>();
        private bool _errorOccurred;

        public Scenario(HttpClientBuilder clientBuilder)
        {
            _clientBuilder = clientBuilder;
        }

        internal Func<IUserAction> When { get; set; }

        public GivenBuilder WithUsers(IEnumerable<User> users)
        {
            _users.AddRange(users);
            return new GivenBuilder(this);
        }
        public GivenBuilder WithUsers(params User[] users)
        {
            return WithUsers((IEnumerable<User>)users);
        }

        internal void AddGiven(Func<IUserAction> action)
        {
            _givens.Add(action);
        }

        internal void AddThens(Func<IUserAction> action)
        {
            _thens.Add(action);
        }

        public async Task Execute(string scenarioName)
        {

            SetupTraceListener();

            var scenarioResult = new ScenarioResult(scenarioName);

            foreach(var user in _users)
            {
                //TODO Erwin this should async, OR authentication is a seperate step to BuildClient()
                user.Initialize(_clientBuilder);
            }
            int index = 1;
            _errorOccurred = false;
            foreach(var given in _givens)
            {
                scenarioResult.Givens.Add(await ExecuteTestStep(given, "Given", index++));
            }
            if(When != null)
            {
                scenarioResult.When = await ExecuteTestStep(When, "When", index++);
            }
            foreach(var then in _thens)
            {
                scenarioResult.Thens.Add(await ExecuteTestStep(then, "Then", index++));
            }

            string failureMessage = null;
            var firstFailedStep = scenarioResult.FirstException();
            if (firstFailedStep != null)
            {
                failureMessage = "** This test failed at step: " + firstFailedStep.StepType + " " + firstFailedStep.StepIndex +
                              " - " +
                              firstFailedStep.Message + Environment.NewLine + Environment.NewLine + "\t Cause: " + firstFailedStep.GetExceptionMessage() + Environment.NewLine;

                Console.WriteLine("**FAILURE *************************************");
                Console.WriteLine(failureMessage);
            }
            Console.WriteLine(scenarioResult.ToString());

            if(firstFailedStep != null)
            {
                throw new InvalidOperationException(failureMessage);
            }
        }

        protected virtual void SetupTraceListener()
        {
            Trace.Listeners.Clear();
            var inMemoryTraceListener = new InMemoryTraceListener();
            Trace.Listeners.Add(inMemoryTraceListener);

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
            foreach(var action in userAction.ResultActions)
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

    public class ScenarioResult
    {
        private readonly string _scenarioName;

        public ScenarioResult(string scenarioName)
        {
            _scenarioName = scenarioName;
            Givens = new List<StepResult>();
            Thens = new List<StepResult>();
        }

        public List<StepResult> Givens { get; private set; }
        public StepResult When { get; set; }
        public List<StepResult> Thens { get; private set; }

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
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("***********************************************");
            builder.AppendLine("Scenario: " + _scenarioName);
            builder.AppendLine("***********************************************");
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

        public int StepIndex { get; private set; }
        public string StepType { get; private set; }
        public List<StepAssertion> StepAssertions { get; private set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public bool HasException()
        {
            return Exception != null || StepAssertions.Any(x => x.Exception != null);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if(HasException())
            {
                builder.Append("....*** Failure in: " + StepType + " " + StepIndex + " - " + FormatMessage(Message) + Environment.NewLine);
            }
            else if(Message != null)
            {
                builder.Append("...." + StepType + " " + StepIndex + " - " + FormatMessage(Message) + Environment.NewLine);
            }

            if(StepAssertions.Any())
            {
                builder.Append("....Assertions:" + Environment.NewLine);
                foreach(var assertion in StepAssertions)
                {
                    if(assertion.Exception != null)
                    {
                        builder.Append("........***Failed: " + StepType + " " + StepIndex + "." + assertion.Index + " - " + FormatAssertionMessage(assertion) +
                                       Environment.NewLine);
                    }
                    else
                    {
                        builder.Append("........" + StepType + " " + StepIndex + "." + assertion.Index + " - " + assertion.Message + Environment.NewLine);
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

    public class InMemoryTraceListener : TraceListener
    {
        private readonly List<string> _messages = new List<string>();
        private StringBuilder _buffer = new StringBuilder();

        public override void Write(string message)
        {
            var currentExecutableFileName = System.IO.Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            if (message.StartsWith(currentExecutableFileName))
            {
                message = message.Substring(currentExecutableFileName.Length).TrimStart();
            }

            _buffer.Append(message);
        }

        public override void WriteLine(string message)
        {
            _buffer.Append(message);
            FlushBuffer();
        }

        private void FlushBuffer()
        {
            if (_buffer.Capacity > 0)
            {
                _messages.Add(_buffer.ToString());
                _buffer.Clear();
            }
        }

        public IEnumerable<string> LogMessages
        {
            get
            {
                FlushBuffer();
                return _messages;
            }
        }
    }

    public class AssertionResult
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}