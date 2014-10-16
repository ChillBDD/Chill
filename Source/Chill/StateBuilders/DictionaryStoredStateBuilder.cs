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
            TestBase.Container.AddToList(valueToSet);
        }

        private void AppendToDictionary(T valueToSet)
        {
            var dictionary = TestBase.Container.Get<Dictionary<string, object>>();

            if (dictionary == null || dictionary.Count == 0)
            {
                dictionary = new Dictionary<string, object>();
            }

            dictionary[Named] = valueToSet;
            TestBase.Container.Set(dictionary);
        }
    }
}