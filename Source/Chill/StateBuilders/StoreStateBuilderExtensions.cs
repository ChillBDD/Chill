namespace Chill.StateBuilders
{
    public static class StoreStateBuilderExtensions
    {
        public static DictionaryStoredStateBuilder<T> Named<T>(this IStoreStateBuilder<T> subject, string name) where T : class
        {
            return new DictionaryStoredStateBuilder<T>(subject.TestBase, name);
        }
    }
}