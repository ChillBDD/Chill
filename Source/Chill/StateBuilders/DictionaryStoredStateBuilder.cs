using System.Collections.Generic;

namespace Chill.StateBuilders
{
    public class DictionaryStoredStateBuilder<T> : StoreStateBuilder<T> where T : class
    {
        public string Named { get; set; }

        public DictionaryStoredStateBuilder(TestBase testBase, string named) : base(testBase)
        {
            Named = named;
        }

        public override TestBase To(T valueToSet)
        {
            AppendToDictionary(valueToSet);

            AppendToList(valueToSet);
            return TestBase;
        }

        private void AppendToList(T valueToSet)
        {
            var list = TestBase.Container.Get<List<T>>();

            if (list == null || list.Count == 0)
            {
                list = new List<T>();
            }

            list.Add(valueToSet);
            TestBase.Container.Set(list);
        }

        private void AppendToDictionary(T valueToSet)
        {
            var dictionary = TestBase.Container.Get<Dictionary<string, T>>();

            if (dictionary == null || dictionary.Count == 0)
            {
                dictionary = new Dictionary<string, T>();
            }

            dictionary[Named] = valueToSet;
            TestBase.Container.Set(dictionary);
        }
    }
}