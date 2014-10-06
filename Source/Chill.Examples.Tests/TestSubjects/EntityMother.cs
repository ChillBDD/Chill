using Ploeh.AutoFixture;

namespace Chill.Examples.Tests.TestSubjects
{
    public static class EntityMother
    {
        private static Fixture fixture = new Fixture();

        public static Customer BuildACustomer()
        {
            return fixture.Create<Customer>();
        }

    }
}