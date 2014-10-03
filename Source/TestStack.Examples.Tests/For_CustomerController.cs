using System;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

using Ploeh.AutoFixture;
using TestStack.Examples.Tests.TestSubjects;
using Xunit;

namespace TestStack.Examples.Tests
{
    namespace For_CustomerController
    {
        [TestClass]
        public class When_deleting_customer : TestFor<CustomerController>
        {
            const int customerId = 12;
            public When_deleting_customer()
            {
                When(() => Subject.Delete(customerId));
            }


            [TestMethod]
            public void Then_model_is_the_existing_custmoer()
            {
                The<ICustomerStore>().Received().DeleteCustomer(customerId);
            }
            
        }

        public class When_retrieving_existing_customer : TestFor<CustomerController, View>
        {
            const int customerId = 12;

            public When_retrieving_existing_customer()
            {
                Given(() =>
                {
                    Store(EntityMother.BuildACustomer()
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
                    Store(EntityMother.BuildACustomer()
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


    public static class EntityMother
    {
        private static Fixture fixture = new Fixture();

        public static Customer BuildACustomer()
        {
            return fixture.Create<Customer>();
        }

    }
}
