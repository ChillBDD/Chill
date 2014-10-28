using Ploeh.AutoFixture;

namespace Chill.Examples.Tests.TestSubjects
{
    public class CustomerMother : ObjectMother<Customer>
    {
        static Fixture fixture = new Fixture();
        protected override Customer Create()
        {
            return fixture.Create<Customer>();
        }
    }
}