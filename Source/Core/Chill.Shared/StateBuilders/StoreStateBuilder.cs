namespace Chill.StateBuilders
{
    /// <summary>
    /// Sets a value in the sate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StoreStateBuilder<T> : IStoreStateBuilder<T>
        where T: class
    {
        public StoreStateBuilder(TestBase testBase)
        {
            TestBase = testBase;
        }


        public virtual TestBase To(T valueToSet)
        {
            TestBase.Container.Set(valueToSet);
            return TestBase;
        }


        public TestBase TestBase { get; set; }
    }
}