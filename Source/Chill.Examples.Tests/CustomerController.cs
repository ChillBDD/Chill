using System.Threading.Tasks;
using Autofac.Core;
using AutofacContrib.NSubstitute;
using Chill.Examples.Tests.TestSubjects;

namespace Chill.Examples.Tests
{
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

        public void Delete(int id)
        {
            this.store.DeleteCustomer(id);
        }

        public async Task<View> GetAsync(int id)
        {
            var customer = await store.GetCustomerAsync(id);

            return new View(customer);

        }


    }

    public class AutofacNSubstituteAutoMockingContainer : IAutoMockingContainer
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
}