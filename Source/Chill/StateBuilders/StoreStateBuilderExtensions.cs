namespace Chill.StateBuilders
{
    public static class StoreStateBuilderExtensions
    {
        public static IndexedStoredStateBuilder<T> AtIndex<T>(this IStoreStateBuilder<T> subject, int index) where T : class
        {
            return new IndexedStoredStateBuilder<T>(subject.TestBase, index);
        }

        public static DictionaryStoredStateBuilder<T> Named<T>(this IStoreStateBuilder<T> subject, string name) where T : class
        {
            return new DictionaryStoredStateBuilder<T>(subject.TestBase, name);
        }
    }
}