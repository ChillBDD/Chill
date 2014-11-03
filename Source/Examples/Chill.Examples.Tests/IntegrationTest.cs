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



    public class IntegrationTests
    {
        [ChillContainer(typeof(AutofacChillContainer))]
        public class MyTest : GivenSubject<Server>
        {
            public MyTest()
            {
                When(() => Subject.Get<UserList>("/api/users"));
            }
        }
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
