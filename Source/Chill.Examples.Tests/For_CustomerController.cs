using Chill;
using Chill.Examples.Tests.TestSubjects;
using Chill.StateBuilders;

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Xunit;

[assembly: ChillContainer(typeof(AutofacNSubstituteChillContainer))]

namespace Chill.Examples.Tests
{

    namespace For_CustomerController
    {



        [TestClass]
        public class When_deleting_customer : GivenSubject<CustomerController>
        {
            const int customerId = 12;
            public When_deleting_customer()
            {
                When(() => Subject.Delete(customerId));
            }

            [TestMethod]
            public void Then_model_is_the_existing_custmoer()
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
                    // Storage for data used in the test. No need to create fields or variables. 
                    SetThe<Customer>().To(EntityMother.BuildACustomer()
                        .With(x => x.Id = customerId));

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
            public void Then_model_is_the_existing_custmoer()
            {
                // One assert per test
                Result.Model.Should().Be(The<Customer>());
            }
        }

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
        }

    }
}
