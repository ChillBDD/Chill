using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Owin;

namespace Chill.Http
{
    //public class SimulatedAuthHttpClientBuilder : HttpClientBuilder
    //{
    //    private readonly List<User> _authenticatedUsers = new List<User>(); 

    //    public SimulatedAuthHttpClientBuilder(string baseAddress, Func<IDictionary<string, object>, Task> appFunc, Func<User, Task> onAuthenticate = null)
    //        : base(BuildSimulateAuthenticationMidFunc(appFunc), onAuthenticate ?? SimulateAuthentication)
    //    {
            
    //    }

    //    private static Func<IDictionary<string, object>, Task> BuildSimulateAuthenticationMidFunc(Func<IDictionary<string, object>, Task> appFunc)
    //    {
    //        var app = new AppBuilder();
    //        app.Use(SimulatedAuthenticationMidFunc());
    //        app.Run(ctx => appFunc(ctx.Environment));
    //        return app.Build();
    //    }

    //    public static Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>> SimulatedAuthenticationMidFunc()
    //    {
    //        return next => ctx =>
    //        {
    //            var owinContext = new OwinContext(ctx);

    //            if (owinContext.Request.Headers.Any(x => x.Key == "simulatedAuthenticationUserName"))
    //            {
    //                var identity = new ClaimsIdentity(new[] { new Claim("name", owinContext.Request.Headers.Get("simulatedAuthenticationUserName")) }, "SimulatedAuthentication");
    //                var principal = new ClaimsPrincipal(identity);

    //                owinContext.Request.User = principal;
                    
    //            }
                
    //            return next(ctx);
    //        };
    //    }

    //    private static Task SimulateAuthentication(User user)
    //    {
    //        user.Client.DefaultRequestHeaders.Add("simulatedAuthenticationUserName", user.Name);
    //        return Task.FromResult(0);
    //    }

    //    public override HttpClient Build(User user)
    //    {
    //        var httpClient = base.Build(user);
    //        return httpClient;
    //    }
    //}
}