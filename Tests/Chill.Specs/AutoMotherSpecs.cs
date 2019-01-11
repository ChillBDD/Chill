using Chill.Specs.TestSubjects;
using FluentAssertions;
using Xunit;

namespace Chill.Specs
{
    public class AutoMotherSpecs : TestBase
    {
        [Fact]
        public void Can_build_a_type_using_automother()
        {
            The<Subject_built_By_Chill_AutoMother>().Name.Should().Be("I have been built by Chill");
        }

        [Fact]
        public void Can_customize_types_built_by_chill()
        {
            The<Subject_built_By_Chill_AutoMother>().Name = "altered";
            The<Subject_built_By_Chill_AutoMother>().Name.Should().Be("altered");
        }

        [Fact]
        public void Can_customize_named_types_built_by_chill()
        {
            TheNamed<Subject_built_By_Chill_AutoMother>("blah").Name = "altered";
            TheNamed<Subject_built_By_Chill_AutoMother>("blah").Name.Should().Be("altered");
        }
    }
}