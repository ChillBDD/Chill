Chill, a BDD style testing framework
=====
##*"If you stick it in a container, Chill will keep it cool."*


* Unit Tests should be maintainable! 
* Unit Tests should be easy to read!
* Unit Tests should hide unneeded complexity!
* Unit Testing should be Cool! I dare say.. it should be Chill!

Chill helps you to write better unit tests. It works wity any test framework, any container, any mocking framework and any assertion library. 

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

This style  a bit of a mixture between MSpec style, BDD Style testing, and using some really useful frameworks such as [XUnit](https://github.com/xunit/xunit), [AutoFac](http://autofac.org/), [NSubstitute](http://nsubstitute.github.io/), [FluentAssertions](http://www.fluentassertions.com/) and [AutoFixture](https://github.com/AutoFixture/AutoFixture). 


## Automocking container

One of the most important ways on how Chill helps with unit testing is by using an auto mocking container. 

Compare the following code snippets:

    [TestMethod]
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
        // Need to add or remove a dependency? prepare yourself to modify ALL your test. 
        var sut = new CustomerController(mockCustomerStore);


        // Call the actual function.. but you also have to create a variable to store your tests. 
        var result = sut.Get(123);

        // Multiple asserts per test? 
        Assert.AreSame(expectedCustomer, result.Model);
        mockCustomerStore.Received().GetCustomer(123);
    }

There are a lot of things wrong with this example:
* The knowledge about which dependencies by your Subject are needed are duplicated among all your tests. 
* In each test, you'll have to explicitly create mock objects. This clutters your test with code that does add any value. 
* Multiple asserts per test make it more difficult to figure out what exactly goes wrong. 
* No explicit structure to this test. What exactly is setup code. 
* Even though most tests use a subject, a result and variables, the naming of these variables will be very different across different tests by different authors, making them more difficult to understand. 

Compare this with a Chill example:

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

                // Subject under test is created automatically and accessable via the Subject property
                When(() => Subject.Get(customerId));
            }

  

            [Fact]
            public void Then_view_is_returned()
            {
                Result.Should().NotBeNull();
            }

            [Fact]
            public void Then_model_is_the_existing_custmoer()
            {
                Result.Model.Should().Be(The<Customer>());
            }
        }
        

The automocking container sets up your **Subject** and automatically injects mock objects for any dependencies it needs. This is great if you have many tests against the same subject and you need to add a dependency. Your tests might fail, but that's exactly what you want. But they still compile and run!

There is no need to explicitly create mock objects anymore. The **The<>** method will create, register and return a mock object for you, ready for use. 

If you want to explicitly register a value, you can use the **SetThe<>().To()** method to an object. There is also a shorthand for this: **UseThe()**. 

Note the use of the **.With()** extension method. This simple little extension method makes it easy to modify objects afther they have been built, in a very clean way. 

##Asynchronous testing
Let’s face it. Async programming is difficult. 

The Async Await certainly helps to make the asynchronous code more readable. Along that lines, Chill attempts to help to make your tests more readable as well. 

Assume the following simple example. You have an asynchronous webapi controller. Why is this controller async? Let’s assume it needs to do IO. In this case, this is encapsulated in an async call to ICustomerStore.GetCustomerAsync(). 

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

                When(() => Subject.GetAsync(customerId));
            }

            [Fact]
            public void Then_view_is_returned()
            {
                Result.Should().NotBeNull();
            }

            [Fact]
            public void Then_model_is_the_existing_custmoer()
            {
                Result.Model.Should().Be(The<Customer>());
            }

In Chill, you can just define an asynchronous method in your call to **When()**. Chill will take care of handling the asynchronous complexity for you. Now all you need to do is to make sure your dependency return a Task instead of the ‘normal’ result. You can do this by calling the **.Asynchronously()** extension method. 



