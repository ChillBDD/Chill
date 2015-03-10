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
    using Http;
    using Xunit;

        using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
    System.Threading.Tasks.Task
    >;


    // Inject HttpClient
    // 

    public class OwinBasedTests : ExampleTestScenarios
    {
        public OwinBasedTests()
        {
            UseThe<Scenario>(new Scenario(new SimulatedAuthHttpClientBuilder("http://localhost", new ExampleAppMiddleware().AppFunc)));
        }
    }

    public static class AppFuncExtensions
    {
        public static HttpClient BuildHttpClient(this AppFunc appFunc)
        {
            var handler = new OwinHttpMessageHandler(appFunc)
            {
                AllowAutoRedirect = true,
                CookieContainer = new CookieContainer(),
                UseCookies = true, 
            };
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            return httpClient;
        }

        
    }


    public abstract class ExampleTestScenarios : TestBase
    {
        public ExampleTestScenarios()
        {
            UseThe<User>(new User("erwin"));
        }
        [Fact]
        public async Task Can_get_test_url()
        {
            await The<Scenario>()
                .WithUsers(All<User>())
                .Given(() => 
                    The<User>().Gets("/test"))
                .Execute();
        }

        [Fact]
        public async Task Can_get_authenticated_url()
        {
            await The<Scenario>()
                .WithUsers(All<User>())
                .Given(() =>
                    The<User>().Gets("/RequiresAuthentication", checkStatusCodeIsSuccess: false)
                        .ResponseShouldMatch(response => response.StatusCode == HttpStatusCode.Unauthorized))
                .When(() => 
                    The<User>().LogsIn())
                .Then(() =>
                    The<User>().Gets("/RequiresAuthentication")
                        .ResponseShouldMatch(response => response.IsSuccessStatusCode))
                .Execute();
        }
    }
}
