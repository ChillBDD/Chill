namespace Chill.ExampleApp
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.Owin;
    using Microsoft.Owin.Builder;
    using Owin;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
    System.Threading.Tasks.Task>;
    using MidFunc = System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>,
        System.Threading.Tasks.Task
        >, System.Func<System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>
        >;

    public class ExampleAppMiddleware
    {
        private Func<IDictionary<string, object>, Task> _appFunc;
        public ExampleAppMiddleware()
        {
            var app = new AppBuilder();

            app.Use(BasicAuth());
            app.UseWebApi(BuildWebApiOptions());
            

            _appFunc = app.Build();
        }

        private HttpConfiguration BuildWebApiOptions()
        {
            var config = new HttpConfiguration()
            {
                
            };
            config.MapHttpAttributeRoutes();
            return config;
        }

        private MidFunc BasicAuth()
        {
            return next => context =>
            {
                var request = new OwinRequest(context);

                var header = request.Headers.Get("Authorization");

                if (!String.IsNullOrWhiteSpace(header))
                {
                    var authHeader = System.Net.Http.Headers
                                       .AuthenticationHeaderValue.Parse(header);

                    if ("Basic".Equals(authHeader.Scheme,
                                             StringComparison.OrdinalIgnoreCase))
                    {
                        string parameter = Encoding.UTF8.GetString(
                                              Convert.FromBase64String(
                                                    authHeader.Parameter));
                        var parts = parameter.Split(':');

                        string userName = parts[0];
                        string password = parts[1];

                        // Just a dumb check if they are equal.. YOU SHOULD REALLY DO SOME BETTER CHECKS HERE
                        // Or even better.. just avoid basic auth and use token based authentication
                        if (userName == password) 
                        {
                            var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, "Badri")
                    };
                            var identity = new ClaimsIdentity(claims, "Basic");

                            request.User = new ClaimsPrincipal(identity);
                        }
                    }
                }

                return next(context);
            };
        }

        public Func<IDictionary<string, object>, Task> AppFunc
        {
            get { return _appFunc; }
        }
    }
}