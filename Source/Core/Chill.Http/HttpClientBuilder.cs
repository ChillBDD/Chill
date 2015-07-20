using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Chill.Http
{
    public class HttpClientBuilder
    {
        private readonly Func<User, Task> _onAuthenticate;
        private readonly Func<IDictionary<string, object>, Task> _appFunc;
        private readonly string _baseAddress;
        private readonly string _url;

        public HttpClientBuilder(string baseAddress, Func<IDictionary<string, object>, Task> appFunc = null, Func<User, Task> onAuthenticate = null)
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
            if (_onAuthenticate == null)
                return Task.FromResult(0);
            return _onAuthenticate(user);
        }
    }
}