using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Chill.Tests.TestSubjects
{
    public class TestClass
    {
        public TestClass()
        {
            
        }

        public TestClass(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        protected bool Equals(TestClass other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestClass) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }

    public interface ITestService
    {
        bool TryMe();
    }

    public class Subject_that_will_be_built_automatically_by_Chill
    {
        public Subject_that_will_be_built_automatically_by_Chill()
        {
        }

        public Subject_that_will_be_built_automatically_by_Chill(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class TestAutoMother : IAutoMother
    {
        public bool Applies(Type type)
        {
            return type == typeof (Subject_that_will_be_built_automatically_by_Chill);
        }

        public T Create<T>(IChillContainer container)
        {
            if (typeof (T) != typeof (Subject_that_will_be_built_automatically_by_Chill))
            {
                throw new InvalidOperationException("This builder can only build Subject_that_will_be_built_automatically_by_Chill");
            }

            return (T)(object) new Subject_that_will_be_built_automatically_by_Chill("I have been built by Chill");
        }
    }

}
