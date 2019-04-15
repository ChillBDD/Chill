namespace Chill.StateBuilders
{
    /// <summary>
    /// Extensions of the State builder
    /// </summary>
    public static class StoreStateBuilderExtensions
    {
        /// <summary>
        /// Allows you to objects in the container under a specified name. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DictionaryStoredStateBuilder<T> Named<T>(this IStoreStateBuilder<T> subject, string name) where T : class
        {
            return new DictionaryStoredStateBuilder<T>(subject.TestBase, name);
        }
    }
}