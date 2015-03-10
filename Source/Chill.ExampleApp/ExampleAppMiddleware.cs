namespace Chill.ExampleApp
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.Owin.Builder;
    using Owin;

    public class ExampleAppMiddleware
    {
        private Func<IDictionary<string, object>, Task> _appFunc;
        public ExampleAppMiddleware()
        {
            var appBuilder = new AppBuilder();

            appBuilder.UseWebApi(BuildWebApiOptions());

            _appFunc = appBuilder.Build();
        }

        private HttpConfiguration BuildWebApiOptions()
        {
            var config = new HttpConfiguration()
            {
                
            };
            config.MapHttpAttributeRoutes();
            return config;
        }

        public Func<IDictionary<string, object>, Task> AppFunc
        {
            get { return _appFunc; }
        }
    }
}