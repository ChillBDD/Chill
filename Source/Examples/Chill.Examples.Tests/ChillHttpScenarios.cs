namespace Chill.Examples.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using ExampleApp;
    using FluentAssertions;
    using Http;
    using Microsoft.Owin.Hosting;
    using Owin;
    using Xunit;

        using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
    System.Threading.Tasks.Task
    >;



    //public class KatanaBasedTests : ExampleTestScenarios
    //{
    //    private IDisposable _webApp;
    //    private string _baseAddress;

    //    public KatanaBasedTests()
    //    {
    //        _baseAddress = "http://localhost:12345";
    //        _webApp = WebApp.Start<Startup>(new StartOptions(_baseAddress) {ServerFactory = "Microsoft.Owin.Host.HttpListener"});

    //        UseThe<Scenario>(new Scenario(new HttpClientBuilder(_baseAddress, onAuthenticate: OnAuthenticate)));
    //    }

    //    private Task OnAuthenticate(User user)
    //    {
    //        var byteArray = Encoding.ASCII.GetBytes("userandpass:userandpass");
    //        user.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

    //        return Task.FromResult(0);
    //    }

    //    protected override void Dispose(bool disposing)
    //    {
    //        _webApp.Dispose();
    //        base.Dispose(disposing);
    //    }
    //}

    //public class Startup
    //{

    //    public void Configuration(IAppBuilder app)
    //    {
    //        var appFunc = new ExampleAppMiddleware().AppFunc;
    //        app.Run(context => appFunc(context.Environment));
    //    }
    //}

    //public class OwinBasedTests : ExampleTestScenarios
    //{
    //    public OwinBasedTests()
    //    {
    //        UseThe<Scenario>(new Scenario(new SimulatedAuthHttpClientBuilder("http://localhost", new ExampleAppMiddleware().AppFunc)));
    //    }
    //}

    //public static class AppFuncExtensions
    //{
    //    public static HttpClient BuildHttpClient(this AppFunc appFunc)
    //    {
    //        var handler = new OwinHttpMessageHandler(appFunc)
    //        {
    //            AllowAutoRedirect = true,
    //            CookieContainer = new CookieContainer(),
    //            UseCookies = true, 
    //        };
    //        var httpClient = new HttpClient(handler)
    //        {
    //            BaseAddress = new Uri("http://localhost/")
    //        };
    //        return httpClient;
    //    }

        
    //}


    //public abstract class ExampleTestScenarios : TestBase
    //{
    //    public ExampleTestScenarios()
    //    {
    //        UseThe<User>(new User("erwin"));
    //    }
    //    [Fact]
    //    public async Task Can_get_test_url()
    //    {
    //        await The<Scenario>()
    //            .WithUsers(All<User>())
    //            .Given(() => 
    //                The<User>().Gets("/test")
    //                .ResponseShouldMatch(r => r.Content.ReadAsStringAsync().Result == "\"Tested Get\""))
    //            .Execute();
    //    }

    //    [Fact]
    //    public async Task Can_get_authenticated_url()
    //    {
    //        await The<Scenario>()
    //            .WithUsers(All<User>())
    //            .Given(() =>
    //                The<User>().Gets("/RequiresAuthentication", checkStatusCodeIsSuccess: false)
    //                    .ResponseShouldMatch(response => response.StatusCode == HttpStatusCode.Unauthorized))
    //            .When(() => 
    //                The<User>().LogsIn())
    //            .Then(() =>
    //                The<User>().Gets("/RequiresAuthentication")
    //                    .ResponseShouldMatch(response => response.IsSuccessStatusCode))
    //            .Execute();
    //    }
    //}
}
