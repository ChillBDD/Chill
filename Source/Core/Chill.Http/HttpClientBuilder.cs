using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Chill.Http
{

    using MidFunc = System.Func<
       System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>,
       System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>;


    public class HttpClientBuilder
    {
        private readonly Func<TestUser, Task> _onAuthenticate;
        private readonly MidFunc _midFunc;
        private readonly string _baseAddress;

        public HttpClientBuilder(string baseAddress, Func<TestUser, Task> onAuthenticate = null)
        {
            _onAuthenticate = onAuthenticate;
            _baseAddress = baseAddress;
        }


        public HttpClientBuilder(MidFunc midFunc, Func<TestUser, Task> onAuthenticate = null)
        {
            _onAuthenticate = onAuthenticate;
            _midFunc = midFunc;
            _baseAddress = "http://localhost";
        }

        public virtual HttpClient Build(TestUser testUser)
        {
            HttpMessageHandler handler;

            if (_midFunc == null)
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
                handler = new OwinHttpMessageHandler(_midFunc)
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

        public Task Authenticate(TestUser testUser)
        {
            if (_onAuthenticate == null)
                return Task.FromResult(0);
            return _onAuthenticate(testUser);
        }
    }
}