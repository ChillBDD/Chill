using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Chill.Autofac;
using Chill.Tests.TestSubjects;
using FakeItEasy;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Chill.Tests
{
    [ChillTestInitializer(typeof(DefaultChillTestInitializer<AutofacFakeItEasyMockingContainer>))]
    public class AutofacFakeItEasySpec : TestBase
    {
        [Fact]
        public void Can_get_FakeItEasy_mock_from_container()
        {
            var testService = The<ITestService>();
            A.CallTo(() => The<ITestService>().TryMe()).Returns(true);

            The<ITestService>().TryMe().Should().Be(true);
        }
    }
}
