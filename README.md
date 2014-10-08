Chill, a BDD style testing framework
=====
##*"If you stick it in a container, Chill will keep it cool."*

The last couple of years, I’ve found myself in a position where I had to ‘sell’ unit testing to my colleagues. Most of them weren't familiar with unit testing, but some were downright against it. So, I tried to work on a way that minimized overhead and maximized readability.
Unit testing, when done correctly, is a great technique to improve the quality of your code and to speed up development.  When done incorrectly, it will take all technical debt and cast them in concrete, grinding development speed down.
To overcome this, I’ve worked quite a bit to come up with a set of techniques to overcome this. Of course, I’m standing on the shoulder of giants here. All I’ve done is to combine several tools, ideas and techniques together into a form that minimizes overhead and maximizes impact.

Here’s an example of my preferred style of tests:

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
        public void Then_model_is_the_existing_custmoer()
        {
            Result.Model.Should().Be(The<Customer>());
        }
    }
}
```

It’s a bit of a mixture between MSpec style, BDD Style testing, and using some really useful frameworks such as [XUnit](https://github.com/xunit/xunit), [AutoFac](http://autofac.org/), [NSubstitute](http://nsubstitute.github.io/), [FluentAssertions](http://www.fluentassertions.com/) and [AutoFixture](https://github.com/AutoFixture/AutoFixture). 
