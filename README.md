[![Build status](https://ci.appveyor.com/api/projects/status/q5ebbayj2yyjavtd/branch/master?svg=true)](https://ci.appveyor.com/project/dennisdoomen/chill-hacqo/branch/develop)
[![](https://img.shields.io/github/release/ChillBDD/Chill.svg?label=latest%20release)](https://github.com/ChillBDD/Chill/releases/latest)
[![](https://img.shields.io/nuget/dt/Chill.svg?label=nuget%20downloads)](https://www.nuget.org/packages/chill)
![](https://img.shields.io/badge/release%20strategy-gitflow-orange.svg)


Chill, a BDD style testing framework
=====
> *"If you stick it in a container, Chill will keep it cool."*

## Why do you need it?

Let's agree on some basic principles.

* Unit Tests should be maintainable! 
* Unit Tests should be easy to read!
* Unit Tests should hide unneeded complexity!
* Unit Testing should be Cool! I dare say.. it should be Chill!

Chill helps you to write better unit tests. It works with any test framework, any container, any mocking framework and any assertion library.

Look at this:

```csharp
namespace For_CustomerController
{
    public class When_retrieving_existing_customer : GivenSubject<CustomerController, View> 
    {
        const int customerId = 12;
        
        public When_retrieving_existing_customer()
        {
            Given(() =>
            {
                SetThe<Customer>().To(EntityMother.CreateACustomer().With(x => x.Id = customerId));
            
                The<ICustomerStore>().GetCustomer(customerId).Returns(The<Customer>());
            });
            
            When(() => Subject.Get(customerId));
        }
        
        [Fact]
        public void Then_view_is_returned()
        {
            Result.Should().NotBeNull();
        }
        
        [Fact]
        public void Then_model_is_the_existing_customer()
        {
            Result.Model.Should().Be(The<Customer>());
        }
    }
}
```

This style is a bit of a mixture between MSpec style, BDD Style testing, and using some really useful frameworks such as [XUnit](https://github.com/xunit/xunit), [AutoFac](http://autofac.org/), [NSubstitute](http://nsubstitute.github.io/), [FluentAssertions](https://www.fluentassertions.com/) and [AutoFixture](https://github.com/AutoFixture/AutoFixture).


## Before and after

One of the most important ways on how Chill helps with unit testing is by using an embedded instance of the [Autofac](https://autofac.org/) container combined with the ability to from automatically detected [Object Mothers](https://martinfowler.com/bliki/ObjectMother.html). 

Indulge us for a moment and study the following example.

```csharp
[Fact]
public void TestWithoutTheChillFactor()
{
    // You have to explicitly create mocks for your dependencies and store them in variables.
    var mockCustomerStore = Substitute.For<ICustomerStore>();

    // Also, you have to explicitly create a variable for your test data. 
    var expectedCustomer = new Customer()
    {
        Name = "Erwin",
        Address = "At home",
        Id = 123
    };
    
    mockCustomerStore.GetCustomer(123).Returns(expectedCustomer);

    // Also, you have to explicitly create the subject under test and insert the mocked dependencies. 
    // Need to add or remove a dependency? Prepare yourself to modify ALL your tests.
    var sut = new CustomerController(mockCustomerStore);

    // Call the actual function.. but you also have to create a variable to store your tests. 
    var result = sut.Get(123);

    // Multiple asserts per test? 
    Assert.AreSame(expectedCustomer, result.Model);
    mockCustomerStore.Received().GetCustomer(123);
}
```

There are a lot of things wrong with this example:
* The knowledge about which dependencies by your _subject-under-test_ are needed is duplicated among all your tests.
* In each test, you'll have to explicitly create mock objects. This clutters your test with code that does not add any value.
* Multiple asserts per test make it more difficult to figure out what exactly goes wrong. 
* No explicit structure to this test. What exactly is setup code. 
* Even though most tests use a subject, a result and variables, the naming of these variables will be very different across different tests by different authors, making them more difficult to understand. 

Compare this with a Chill example:

```csharp
public class When_retrieving_existing_customer : GivenSubject<CustomerController, View>
{
    const int customerId = 12;

    public When_retrieving_existing_customer()
    {
        Given(() =>
        {
            // Storage for data used in the test. No need to create fields or variables. 
            SetThe<Customer>().To(EntityMother.BuildACustomer()
                .With(x => x.Id = customerId));

            The<ICustomerStore>().GetCustomer(customerId).Returns(The<Customer>());
        });

        // Subject under test is created automatically and accessible via the Subject property.
        When(() => Subject.Get(customerId));
    }

    [Fact]
    public void Then_view_is_returned()
    {
        Result.Should().NotBeNull();
    }

    [Fact]
    public void Then_model_is_the_existing_customer()
    {
        Result.Model.Should().BeSameAs(The<Customer>());
    }
}
```       

The built-in container sets up your `Subject` and automatically injects the dependencies it needs using the objects you registered using `SetThe` and `To` or its shorthand `UseThe`. And if any of the registered objects or the subject itself implements `IDisposable`, Chill will ensure they get disposed at the end of the test. 

Also notice the use of the `.With()` extension method. This simple little extension method makes it easy to modify objects after they have been built, in a very clean fluent way. Instead of `With`, you can also use `And` to make your modifications look even more fluent.

You don't need to use the `Subject` property if you don't want to. Just use the `GivenWhenThen<TResult>` base-class instead of `GivenSubject<TResult>`. And both classes are available with and without the `Result` property. Just omit the generic type parameter if you don't need it. 

If you need to take control of how the subject is build, just call the `WithSubject` method from your `Given` body directly and use provide a factory method instead. 

```csharp
Given(() =>
{
    WithSubject(resolver => new CustomerController());
});
```

If you want, you can use the provided `resolver` parameter to retrieve the relevant objects from your container. 

## Object Mothers (a.k.a. Auto-Mocking)

You may have noticed that we use `The<ICustomerStore>()` without us registering any implementation. This is possible using the magic of Chill's object mothers. Simply create an implementation of the `IObjectMother` interface and use your favorite mocking library to generate a mock implementation. For instance, this object mother will use NSubstitute. 

```csharp
public class CustomerStoreMother : IObjectMother
{
    public bool IsFallback { get; } = false;

    public bool Applies(Type type) => typeof(Foo).IsAssignableFrom(type);

    public object Create(Type type, IChillObjectResolver resolver)
    {
        return Substitute.For<ICustomerStore>();
    }
}
```

Chill will only scan the assembly in which the tests using Chill live. And the `IsFallback` property allows you to build so-called generic object mother that can build lots of different objects without interfering with more specific object mothers. This is how the [Chill.NSubstitute]() and the [Chill.FakeItEasy]() packages work and why they are content-only packages. 

For your convenience, the abstract `ObjectMother<TTarget>` class can be used to simplify creation of object mothers.

## Asynchronous Testing

Let’s face it; asynchronous programming is difficult. C#'s `async` / `await` keywords certainly help to make asynchronous code more readable. Along those lines, Chill attempts to help to make your tests more readable as well.

Assume the following simple example. You have an asynchronous Web API controller. Why is this controller asynchronous? Let’s assume it needs to do I/O. In this case, this is encapsulated in an `async` call to `ICustomerStore.GetCustomerAsync()`. 

```csharp
public class When_retrieving_existing_customer_async : GivenSubject<CustomerController, View>
{
    const int customerId = 12;

    public When_retrieving_existing_customer_async()
    {
        Given(() =>
        {
            SetThe<Customer>().To(EntityMother.BuildACustomer()
                .With(x => x.Id = customerId));

            The<ICustomerStore>()
                .GetCustomerAsync(customerId)
                .Returns(The<Customer>().Asynchronously());
        });

        When(async () => await Subject.GetAsync(customerId));
    }

    [Fact]
    public void Then_view_is_returned()
    {
        Result.Should().NotBeNull();
    }

    [Fact]
    public void Then_model_is_the_existing_customer()
    {
        Result.Model.Should().Be(The<Customer>());
    }
}
```

In Chill, you can define an asynchronous method in your call to `Given` or `When()`. Chill will take care of handling the asynchronous complexity for you. Now all you need to do is to make sure your dependency returns a `Task` instead of the ‘normal’ result.  

## Catching Exceptions
Chill has built-in support for intercepting the exceptions that the body of your `When` may throw. If you expect an exception, you can tell Chill to defer execution of that body to your assertion code:

```csharp
public class When_a_deferred_async_act_throws_in_a_test_with_subject : GivenSubject<object>
{
    public When_a_deferred_async_act_throws_in_a_test_with_subject()
    {
        When(async () =>
        {
            await Task.Delay(10);
            throw new ApplicationException();
        },
        deferredExecution: true);
    }

    [Fact]
    public void Then_the_exception_should_be_observed()
    {
        // This will trigger the body of the When() call to be executed.
        WhenAction.Should().Throw<ApplicationException>();
    }
}
```

As a short-hand of `When(() => {}, deferredExecution: true)`, you can also use `WhenLater()`.

## Extensibility

Chill uses a version of Autofac that is merged into the Chill's main assembly and it will serve the needs of most users. But if you want, you _can_ use your own container. Just build your own implementation of the `IChillContainer` interface and add the following somewhere to your test project:

```csharp
[assembly: ChillContainer(typeof(MyCustomChillContainer))]
```