using Chill.Examples.Tests.TestSubjects;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Xunit;

namespace Chill.Examples.Tests
{
    namespace For_CustomerController
    {
        [TestClass]
        public class When_deleting_customer : TestFor<CustomerController>
        {
            const int customerId = 12;
            public When_deleting_customer()
            {
                Given(() =>
                {
                });

                When(() => Subject.Delete(customerId));
            }


            [TestMethod]
            public void Then_model_is_the_existing_custmoer()
            {
                this.The<ICustomerStore>().Received().DeleteCustomer(customerId);
            }
            
        }

        public class When_retrieving_existing_customer : TestFor<CustomerController, View>
        {
            const int customerId = 12;

            public When_retrieving_existing_customer()
            {
                Given(() =>
                {
                    SetThe<Customer>().To(EntityMother.BuildACustomer()
                        .With(x => x.Id = customerId));

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

        public class When_retrieving_existing_customer_async : TestFor<CustomerController, View>
        {
            const int customerId = 12;

            public When_retrieving_existing_customer_async()
            {
                Given(() =>
                {
                    SetThe<Customer>().To(EntityMother.BuildACustomer()
                        .With(x => x.Id = customerId));

                    SetThe<Customer>().AtIndex(1).To(EntityMother.BuildACustomer());

                    The<ICustomerStore>()
                        .GetCustomerAsync(customerId)
                        .Returns(The<Customer>().Asynchronously());
                });

                When(() => Subject.GetAsync(customerId));
            }

            [Fact]
            public void Then_the_second_customer_is_set()
            {
                The<Customer>(atIndex: 1).Should().NotBeNull();
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
