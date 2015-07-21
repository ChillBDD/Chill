using Chill.Http.Logging;

namespace Chill.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using KellermanSoftware.CompareNetObjects;
    using Newtonsoft.Json;
    using PowerAssert;

    public class HttpBasedUserAction<TResult> : HttpBasedUserAction, IUserAction<TResult>
    {
        private static ILog s_log = LogProvider.GetCurrentClassLogger();
        private readonly Func<HttpResponseMessage, TResult> _deSerialize;

        private readonly Lazy<TResult> _result;

        public HttpBasedUserAction(string message, TestUser testUser, HttpRequestMessage request
                                   , bool checkStatusCodeIsSuccess = false,
                                   Func<HttpResponseMessage, TResult> deSerialize = null, string eTag = null)
            : base(message, testUser, request, checkStatusCodeIsSuccess, eTag)
        {
            _deSerialize = deSerialize ?? Deserialize;
            _result = new Lazy<TResult>(() => _deSerialize(Response));
        }

        public override async Task Execute()
        {
            await base.Execute();
        }

        private TResult Deserialize(HttpResponseMessage arg)
        {
            return JsonConvert.DeserializeObject<TResult>(arg.Content.ReadAsStringAsync().Result);
        }

        public HttpBasedUserAction<TResult> Do(Action<Response<TResult>> action)
        {
            _resultActions.Add(new ResponseAction(null, () => action(new Response<TResult>(Response, _result.Value))));
            return this;
        }

        public HttpBasedUserAction<TResult> HttpResponseShouldMatch(
            Expression<Func<HttpResponseMessage, bool>> assertion)
        {
            base.ResponseShouldMatch(assertion);
            return this;
        }

        public HttpBasedUserAction<TResult> ForResult(Expression<Action<TResult>> actionExpression)
        {
            var action = actionExpression.Compile();
            _resultActions.Add(new ResponseAction(actionExpression.Humanize(), () => action(_result.Value)));
            return this;
        }

        public HttpBasedUserAction<TResult> ShouldReturnEtag(Action<string> captureEtag = null)
        {
            var assertionText = "Response should return Etag httpheader";
            _resultActions.Add(new ResponseAction(assertionText, () =>
            {
                if(Response.Headers.ETag == null)
                {
                    throw new TestFailedException("Response did not return eTag");
                }

                if(captureEtag != null)
                {
                    captureEtag(Response.Headers.ETag.Tag.Trim('"'));
                }
            }));
            return this;
        }


        public HttpBasedUserAction<TResult> ResultShouldMatch(Expression<Func<TResult, bool>> actionExpression)
        {
            var assertionText = actionExpression.Humanize();
            _resultActions.Add(new ResponseAction(assertionText, () =>
            {
                var method = PartialApplicationVisitor.Apply(actionExpression, _result.Value);
                PAssert.IsTrue(method);
            }));
            return this;
        }

        public HttpBasedUserAction<TResult> ResultPartEquals<T>(Expression<Func<TResult, T>> getter, T expectedValue)
        {
            var assertionText = getter.Humanize() + " should equal " + expectedValue;

            _resultActions.Add(new ResponseAction(assertionText, () =>
            {
                T valueToCheck;
                try
                {
                    valueToCheck = getter.Compile()(_result.Value);
                }
                catch(Exception ex)
                {
                    throw new InvalidOperationException(
                        string.Format("Attempting to execute expression {0} failed", getter), ex);
                }

                var compareLogic = new CompareLogic();

                var result = compareLogic.Compare(expectedValue, valueToCheck);
                if(!result.AreEqual)
                {
                    throw new InvalidOperationException(result.DifferencesString + "\r\n" +
                                                        JsonConvert.SerializeObject(valueToCheck, Formatting.Indented));
                }
            }));
            return this;
        }

        public HttpBasedUserAction<TResult> ResultEquals(TResult expected)
        {
            var assertionText = "The " + typeof(TResult).Name + " result should equal " + expected;

            _resultActions.Add(new ResponseAction(assertionText, () =>
            {
                var compareLogic = new CompareLogic();

                var compareResult = compareLogic.Compare(expected, _result.Value);
                if(!compareResult.AreEqual)
                {
                    throw new InvalidOperationException(compareResult.DifferencesString + "\r\n" +
                                                        JsonConvert.SerializeObject(_result.Value, Formatting.Indented));
                }
            }));
            return this;
        }
    }

    public class HttpBasedUserAction : IUserAction
    {
        private readonly HttpRequestMessage _request;
        protected readonly List<ResponseAction> _resultActions = new List<ResponseAction>();
        private string _eTag;


        public HttpBasedUserAction(string message, TestUser testUser, HttpRequestMessage r, bool checkStatusCode = true,
                                   string eTag = null)
        {
            _request = r;

            if(r.RequestUri.ToString().Contains("#"))
            {
                throw new TestFailedException($"The request URI '{r.RequestUri}' contains unencoded data");
            }

            _eTag = eTag;

            if(eTag != null)
            {
                message += " with etag " + eTag;
                r.Headers.IfNoneMatch.Add(new EntityTagHeaderValue("\"" + eTag + "\""));
            }

            Message = message;
            TestUser = testUser;

            if(checkStatusCode)
            {
                _resultActions.Add(new ResponseAction("Request was handled succesfully by server", () =>
                {
                    if(!Response.IsSuccessStatusCode && Response.StatusCode != HttpStatusCode.NotModified)
                    {
                        throw new TestFailedException(
                            string.Format("The server did not respond with a success Status code, but with {0}, {1}",
                                Response.StatusCode, Response.Content.ReadAsStringAsync().Result));
                    }
                }));
            }
        }

        public HttpBasedUserAction Do(Action<HttpResponseMessage> action)
        {
            _resultActions.Add(new ResponseAction(null, () => action(this.Response)));
            return this;
        }

        public TestUser TestUser { get; }
        public HttpResponseMessage Response { get; private set; }
        public string Message { get; }

        public IEnumerable<ResponseAction> ResultActions
        {
            get { return _resultActions; }
        }

        public virtual async Task Execute()
        {
            Trace.WriteLine(Message + $" using {_request.Method} on {_request.RequestUri}");
            Response = await TestUser.Client.SendAsync(_request);
        }

        public HttpBasedUserAction ResponseShouldMatch(Expression<Func<HttpResponseMessage, bool>> actionExpression)
        {
            var assertionText = actionExpression.Humanize();
            _resultActions.Add(new ResponseAction(assertionText, () =>
            {
                var method = PartialApplicationVisitor.Apply(actionExpression, Response);
                PAssert.IsTrue(method);
            }));
            return this;
        }
    }

    //brought in to avoid the need for .NET 4
}