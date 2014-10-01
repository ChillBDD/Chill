using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using NSubstitute;

using Ploeh.AutoFixture;

using Xunit;

namespace TestStack.Examples.Tests
{
    namespace For_CustomerController
    {
        public class When_retrieving_existing_customer : TestFor<CustomerController, View>
        {
            const int customerId = 12;

            public When_retrieving_existing_customer()
            {
                Given(() =>
                {
                    Store(EntityMother.CreateACustomer()
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

        public class When_retrieving_existing_customer_async : TestFor<CustomerController, Task<View>>
        {
            const int customerId = 12;

            public When_retrieving_existing_customer_async()
            {
                Given(() =>
                {
                    Store(EntityMother.CreateACustomer()
                        .With(x => x.Id = customerId));

                    The<ICustomerStore>().GetCustomer(customerId).Returns(The<Customer>());
                });

                When(() => Subject.GetAll(customerId));
            }

            [Fact]
            public async Task Then_view_is_returned()
            {
                (await Result).Should().NotBeNull();
            }

            [Fact]
            public async Task Then_model_is_the_existing_custmoer()
            {
                (await Result).Model.Should().Be(The<Customer>());
            }
        }

        public class When_retrieving_existing_customer_with_other_exists : TestFor<CustomerController, View>
        {
            const int customerId = 12;

            public When_retrieving_existing_customer_with_other_exists()
            {
                Given(() =>
                {
                    StoreAtIndex(0, EntityMother.CreateACustomer()
                        .With(x => x.Id = customerId));

                    StoreAtIndex(1, EntityMother.CreateACustomer());

                    The<ICustomerStore>().GetCustomer(customerId).Returns(GetFromIndex<Customer>(0));
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
                Result.Model.Should().Be(GetFromIndex<Customer>(0));
            }

            [Fact]
            public void Then_model_is_not_the_wrong_existing_custmoer()
            {
                Result.Model.Should().Be(GetFromIndex<Customer>(1));
            }
        }

    }


    public class CustomerController
    {
        private readonly ICustomerStore store;

        public CustomerController(ICustomerStore store)
        {
            this.store = store;
        }

        public View Get(int id)
        {
            var customer = store.GetCustomer(id);

            return new View(customer);
        }

        public async Task<View> GetAll(int id)
        {
            var customer = store.GetCustomer(id);

            return new View(customer);
        }


    }

    public class View
    {
        public object Model { get; set; }

        public View(object model)
        {
            Model = model;
        }
    }

    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
    }

    public interface ICustomerStore
    {
        Customer GetCustomer(int id);
    }

    public static class EntityMother
    {
        private static Fixture fixture = new Fixture();

        public static readonly Func<Customer> CreateACustomer = () => fixture.Create<Customer>();
    }
}
