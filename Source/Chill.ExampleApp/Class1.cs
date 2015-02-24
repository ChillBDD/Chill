namespace Chill.ExampleApp
{
    using System.Web.Http;
    using Microsoft.Owin.Builder;
    using Owin;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
    System.Threading.Tasks.Task
    >;

    using MidFunc = System.Func<
        System.Func<
            System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task
        >,
        System.Func<
            System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task
        >
        >;

    public class ExampleAppMiddleware
    {
        private AppFunc _appFunc;
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

        public AppFunc AppFunc
        {
            get { return _appFunc; }
        }
    }

    public class TestController : ApiController
    {
        [HttpGet]
        [Route("test")]
        public string Get()
        {
            return "Tested Get";
        }

        [HttpPost]
        [Route("test")]
        public string Post()
        {
            return "Tested Post";
        }
    }
}
