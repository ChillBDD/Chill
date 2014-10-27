using Autofac;
using Chill.Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Chill.Examples.Tests.TestSubjects;
using Autofac.Core.Registration;
namespace Chill.Examples.Tests
{


    public class AutofacChillContainerSpecs
    {
        
        [ChillTestInitializer(typeof(DefaultChillTestInitializer<AutofacContainerWithCustomModule>))]
        public class When_configuring_container_with_module_and_objectmother : GivenWhenThen
        {
            [Fact]
            public void Then_testservice_is_registered_through_module()
            {
                The<ITestService>().GetType().Should().Be(typeof(TestService));
            }

            [Fact]
            public void Then_customer_is_resolved_through_Customer_Automother()
            {
                The<Customer>().Name.Should().NotBeNull();
            }

            [Fact]
            public void Then_cannot_resolve_unknown_interface()
            {
                Action a = () => The<IAppDomainSetup>();

                a.ShouldThrow<ComponentNotRegisteredException>();
            }


        }

        /// <summary>
        /// You can define 'non mocking' with all the type registrations that you would otherwise in your application. 
        /// This can either be done in the App
        /// </summary>
        internal class AutofacContainerWithCustomModule : AutofacChillContainer
        {
            public AutofacContainerWithCustomModule()
                : base(CreateContainerBuilder())
            {

            }

            private static ContainerBuilder CreateContainerBuilder()
            {
                var builder = new ContainerBuilder();
                builder.RegisterModule<CustomAutofacModule>();
                return builder;
            }
        }



        public class CustomAutofacModule  : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<TestService>().As<ITestService>();
                base.Load(builder);
            }
        }

    }

    public class IntegrationTests
    {
        [ChillTestInitializer(typeof(DefaultChillTestInitializer<AutofacChillContainer>))]
        public class MyTest : GivenSubject<Server>
        {
            public MyTest()
            {
                When(() => Subject.Get<UserList>("/api/users"));
            }

            
            
        }
    }


    
    public interface ITestService
    {

    }
    public class TestService : ITestService
    {

    }

    public class ServerMother : ObjectMother<Server>
    {
        protected override Server Create()
        {
            var server = new Server();
            server.Open();

            return server;
        }
        
    }

    public class UserList : List<User>
    {

    }

    public class User
    {

    }

    public class Server : IDisposable
    {
        public bool IsOpened { get; private set; }
        public string Get(string url)
        {
            return "";
        }

        public T Get<T>(string url)
        {
            return default(T);
        }
        
        public void Close()
        {

        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        internal void Open()
        {
            IsOpened = true;
        }
    }
}
