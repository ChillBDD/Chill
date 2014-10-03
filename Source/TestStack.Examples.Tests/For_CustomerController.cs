using System;
using System.Threading.Tasks;

using FluentAssertions;

using NSubstitute;

using Ploeh.AutoFixture;
using AutofacContrib.NSubstitute;
using TestStack.Examples.Tests.TestSubjects;
using Xunit;

namespace TestStack.Examples.Tests
{
    public class AutoFacNSubstituteAutoMockingContainer : IAutoMockingContainer
    {
        private AutoSubstitute container = new AutoSubstitute();
        public T Get<T>() where T : class
        {
            return container.Resolve<T>();
        }

        public T Set<T>(T valueToSet) where T : class
        {
            return container.Provide(valueToSet);

        }

        public void Dispose()
        {
            container.Dispose();
        }
    }


    namespace For_CustomerController
    {
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

                When(() =>
                {
                    return Subject.Get(customerId);
                });
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
                    Store(EntityMother.BuildACustomer()
                        .With(x => x.Id = customerId));

                    The<ICustomerStore>().GetCustomer(customerId).Returns(The<Customer>());
                });

                When(() => Subject.GetAsync(customerId));
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
