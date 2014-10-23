using Chill.Examples.Tests.TestSubjects;
using NSubstitute;
using Xunit;


namespace Chill.Examples.Tests
{
    class NonChilledTest
    {
        [Fact]
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
            Assert.Same(expectedCustomer, result.Model);
            mockCustomerStore.Received().GetCustomer(123);
        }
    }
}