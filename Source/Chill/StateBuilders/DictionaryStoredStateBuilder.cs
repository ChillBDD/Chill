using System;
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
            var dictionary = TestBase.Container.Get<Dictionary<Tuple<Type, string>, object>>();

            var key = Tuple.Create(typeof(T), Named);

            if (dictionary == null || dictionary.Count == 0)
            {
                dictionary = new Dictionary<Tuple<Type, string>, object>();
            }

            dictionary[key] = valueToSet;
            TestBase.Container.Set(dictionary);
        }
    }
}