using System.Threading.Tasks;
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

}