using System;
using System.Dynamic;
using FluentAssertions;
using Xunit;

namespace Chill.Specs
{
    public class AutoMotherSpecs : GivenWhenThen
    {
        [Fact]
        public void When_only_a_fallback_mother_exists_for_a_type_it_should_use_the_fallback()
        {
            var actual = The<IFoo>();
            actual.Should().NotBeNull();
            actual.Creator.Should().Be(nameof(FallbackFooAutoMother));
        }

        [Fact]
        public void When_a_more_specific_mother_exists_for_a_type_it_should_use_the_specific_mother()
        {
            var actual = The<Foo>();
            actual.Should().NotBeNull();
            actual.Creator.Should().Be(nameof(SpecificFooAutoMother));
        }

        [Fact]
        public void When_multiple_objects_are_requested_using_the_same_type_it_should_always_return_the_same_object()
        {
            The<Foo>().Should().BeSameAs(The<Foo>());
        }

        [Fact]
        public void When_an_instance_was_already_registered_it_should_not_use_any_object_mothers()
        {
            var expected = UseThe<IFoo>(new Foo());
            var actual = The<IFoo>();

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void When_an_instance_was_already_registered_multiple_requests_should_return_the_same_object()
        {
            UseThe<IFoo>(new Foo());
            
            The<IFoo>().Should().BeSameAs(The<IFoo>());
        }
        
        [Fact]
        public void When_a_named_object_is_requested_multiple_times_it_should_return_the_same_object()
        {
            TheNamed<IFoo>("name").Should().BeSameAs(TheNamed<IFoo>("name"));
        }

        [Fact]
        public void When_differently_named_objects_are_requested_it_should_return_different_objects()
        {
            TheNamed<IFoo>("name").Should().NotBeSameAs(TheNamed<IFoo>("otherName"));
        }

        [Fact]
        public void When_more_than_one_specific_object_mother_exists_for_the_requested_type()
        {
            Action act = () => The<ObjectWithDuplicateMothers>();
            act.Should().Throw<InvalidOperationException>().WithMessage("*more than one builders*");
        }
    }

    public interface IFoo
    {
        string Creator { get; set; }
    }

    public class Foo : IFoo
    {
        public string Creator { get; set; }
    }

    public class FallbackFooAutoMother : IObjectMother
    {
        public bool IsFallback { get; } = true;

        public bool Applies(Type type)
        {
            return typeof(IFoo).IsAssignableFrom(type);
        }

        public object Create(Type type, IChillObjectResolver objectResolver)
        {
            return new Foo
            {
                Creator = nameof(FallbackFooAutoMother)
            };
        }
    }

    public class SpecificFooAutoMother : IObjectMother
    {
        public bool IsFallback { get; } = false;

        public bool Applies(Type type)
        {
            return typeof(Foo).IsAssignableFrom(type);
        }

        public object Create(Type type, IChillObjectResolver objectResolver)
        {
            return new Foo
            {
                Creator = nameof(SpecificFooAutoMother)
            };
        }
    }

    public class DuplicateAutoMother1 : ObjectMother<ObjectWithDuplicateMothers>
    {
        protected override ObjectWithDuplicateMothers Create()
        {
            return new ObjectWithDuplicateMothers();
        }
    }

    public class DuplicateAutoMother2 : ObjectMother<ObjectWithDuplicateMothers>
    {
        protected override ObjectWithDuplicateMothers Create()
        {
            return new ObjectWithDuplicateMothers();
        }
    }

    public class ObjectWithDuplicateMothers
    {
    }
}
