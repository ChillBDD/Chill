using Chill.Tests.TestSubjects;

using FluentAssertions;

using Xunit;

namespace Chill.Tests.CoreScenarios
{
    public abstract class TestBaseSpecs : TestBase
    {
        [Fact]
        public void Can_get_mock_for_subject()
        {
            The<ITestService>().Should().NotBeNull();
        }

        [Fact]
        public void Can_get_named_service()
        {
            TheNamed<ITestService>("abc").Should().NotBeNull();
        }

        [Fact]
        public void Can_register_object()
        {
            UseThe(new TestClass("abc"));
            The<TestClass>().Name.Should().Be("abc");
        }

        [Fact]
        public void Can_register_named_object()
        {
            UseThe(new TestClass("abc"));
            The<TestClass>().Name.Should().Be("abc");
        }

        [Fact]
        public void Can_register_object_fluently()
        {
            SetThe<TestClass>().To(new TestClass("abc"));
            The<TestClass>().Name.Should().Be("abc");
        }
    }
}