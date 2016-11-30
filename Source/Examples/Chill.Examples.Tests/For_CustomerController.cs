using Chill;
using Chill.Autofac;
using Chill.Examples.Tests.TestSubjects;
using FluentAssertions;
using NSubstitute;
using Xunit;



namespace Chill.Examples.Tests
{

    namespace For_CustomerController
    {
        using AutofacNSubstitute;


        public class When_deleting_customer : GivenSubject<CustomerController>
        {
            const int customerId = 12;
            public When_deleting_customer()
            {
                When(() => Subject.Delete(customerId));
            }

            [Fact]
            public void Then_model_is_the_existing_customer()
            {
                this.The<ICustomerStore>().Received().DeleteCustomer(customerId);
            }

            
        }

        public class When_retrieving_existing_customer : GivenSubject<CustomerController, View>
        {
            const int customerId = 12;

            public When_retrieving_existing_customer()
            {
                // Explicit phases for setup
                Given(() =>
                {
                    The<Customer>()
                        .With(x => x.Id = customerId);

                    // Automatic creating of mock objects. Here using NSubstitute as a friendly mocking framework
                    The<ICustomerStore>().GetCustomer(customerId).Returns(The<Customer>());
                    
                });

                // Subject under test is created automatically and accessable via the Subject property
                When(() => Subject.Get(customerId));
            }

  

            [Fact]
            public void Then_view_is_returned()
            {
                // Result property is created automatically, if needed and allows type safe access
                Result.Should().NotBeNull();
            }

            [Fact]
            public void Then_model_is_the_existing_customer()
            {
                // One assert per test
                Result.Model.Should().Be(The<Customer>());
            }
        }

        [ChillContainer(typeof(AutofacNSubstituteChillContainer))]
        public class When_retrieving_existing_customer_async : GivenSubject<CustomerController, View>
        {
            const int customerId = 12;

            public When_retrieving_existing_customer_async()
            {
                Given(() =>
                {
                    The<Customer>()
                        .With(x => x.Id = customerId);

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
            public void Then_model_is_the_existing_customer()
            {
                Result.Model.Should().Be(The<Customer>());
            }
        }

    }
}
