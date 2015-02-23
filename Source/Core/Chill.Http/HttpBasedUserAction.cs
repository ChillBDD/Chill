namespace Chill.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Net.Http;
    using KellermanSoftware.CompareNetObjects;
    using PowerAssert;
    

    public class HttpBasedUserAction<TResult> : HttpBasedUserAction, IUserAction<TResult>
    {
        private readonly Func<HttpResponseMessage, TResult> _deSerialize;
        private TResult _result;

        public HttpBasedUserAction(string message, User user, HttpRequestMessage request, Func<HttpResponseMessage, TResult> deSerialize,
            bool checkStatusCodeIsSuccess)
            : base(message, user, request, checkStatusCodeIsSuccess)
        {
            _deSerialize = deSerialize;
        }

        public HttpBasedUserAction<TResult> Do(Action<Response<TResult>> action)
        {
            _resultActions.Add(new ResponseAction(null, () => action(new Response<TResult>(Response, _result))));
            return this;
        }

        public new HttpBasedUserAction<TResult> ResponseShouldMatch(Expression<Func<HttpResponseMessage, bool>> assertion)
        {
            base.ResponseShouldMatch(assertion);
            return this;
        }

        public HttpBasedUserAction<TResult> ForResult(Expression<Action<TResult>> actionExpression)
        {
            var action = actionExpression.Compile();
            _resultActions.Add(new ResponseAction(actionExpression.Humanize(), () => action(_result)));
            return this;
        }

        public HttpBasedUserAction<TResult> ResultShouldMatch(Expression<Func<TResult, bool>> actionExpression)
        {
            string assertionText = actionExpression.Humanize();
            _resultActions.Add(new ResponseAction(assertionText, () =>
            {
                var method = PartialApplicationVisitor.Apply(actionExpression, _result);
                PAssert.IsTrue(method);
            }));
            return this;
        }

        public HttpBasedUserAction<TResult> ResultPartEquals<T>(Expression<Func<TResult, T>> getter, T expectedValue)
        {
            string assertionText = getter.Humanize() + " should equal " + expectedValue;

            _resultActions.Add(new ResponseAction(assertionText, () =>
            {
                T valueToCheck;
                try
                {
                    valueToCheck = getter.Compile()(_result);
                }
                catch(Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Attempting to execute expression {0} failed", getter), ex);
                }

                CompareLogic compareLogic = new CompareLogic();

                var result = compareLogic.Compare(valueToCheck, expectedValue);
                if(!result.AreEqual)
                {
                    throw new InvalidOperationException(result.DifferencesString);
                }
            }));
            return this;
        }

        public HttpBasedUserAction<TResult> ResultEquals(TResult result)
        {
            string assertionText = "The " + typeof(TResult).Name + " result should equal " + result;

            _resultActions.Add(new ResponseAction(assertionText, () =>
            {
                CompareLogic compareLogic = new CompareLogic();

                var compareResult = compareLogic.Compare(_result, result);
                if(!compareResult.AreEqual)
                {
                    throw new InvalidOperationException(compareResult.DifferencesString);
                }
            }));
            return this;
        }

        public override void Execute()
        {
            base.Execute();

            _result = _deSerialize(Response);
        }
    }

    public class HttpBasedUserAction : IUserAction
    {
        private readonly HttpRequestMessage _request;
        protected readonly List<ResponseAction> _resultActions = new List<ResponseAction>();

        public HttpBasedUserAction(string message, User user, HttpRequestMessage r, bool checkStatusCode = true)
        {
            _request = r;
            Message = message;
            User = user;

            if(checkStatusCode)
            {
                ResponseShouldMatch(httpresponse => httpresponse.IsSuccessStatusCode);
            }
        }

        public User User { get; private set; }
        public HttpResponseMessage Response { get; private set; }
        public string Message { get; private set; }

        public IEnumerable<ResponseAction> ResultActions
        {
            get { return _resultActions; }
        }

        public virtual void Execute()
        {
            // Todo: solve logging story
            //Console.WriteLine((Message + " using {Method} on {Url}", _request.Method, _request.RequestUri);
            Response = User.Client.SendAsync(_request).Result;
        }

        public HttpBasedUserAction ResponseShouldMatch(Expression<Func<HttpResponseMessage, bool>> actionExpression)
        {
            string assertionText = actionExpression.Humanize();
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