namespace Chill.StateBuilders
{
    /// <summary>
    /// Interface for 'storestatebuilders'. This is the fluent syntax for storing state in the container
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStoreStateBuilder<T>
    {
        /// <summary>
        /// Testclass that's used in the tests. 
        /// </summary>
        TestBase TestBase { get; set; }

        /// <summary>
        /// Set the specified value in the container. 
        /// </summary>
        /// <param name="valueToSet"></param>
        /// <returns></returns>
        TestBase To(T valueToSet);

    }
}