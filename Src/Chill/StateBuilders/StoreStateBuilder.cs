namespace Chill.StateBuilders
{
    /// <summary>
    /// The state builder provides a fluent syntax for writing values in the container
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StoreStateBuilder<T> : IStoreStateBuilder<T>
        where T: class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreStateBuilder{T}"/> class.
        /// </summary>
        /// <param name="testBase">The test base.</param>
        public StoreStateBuilder(TestBase testBase)
        {
            TestBase = testBase;
        }


        /// <summary>
        /// Write a value in the container
        /// </summary>
        /// <param name="valueToSet"></param>
        /// <returns></returns>
        public virtual TestBase To(T valueToSet)
        {
            TestBase.Decorator.Set(valueToSet);
            return TestBase;
        }

        /// <summary>
        /// A reference to the test class
        /// </summary>
        public TestBase TestBase { get; set; }
    }
}