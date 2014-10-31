using System;
using System.Collections.Generic;

namespace Chill.StateBuilders
{
    /// <summary>
    /// Allows you to set a named value the container using a fluent syntax. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DictionaryStoredStateBuilder<T> : StoreStateBuilder<T> where T : class
    {
        /// <summary>
        /// The name specified
        /// </summary>
        public string Named { get; set; }

        /// <summary>
        /// Creates a <see cref="DictionaryStoredStateBuilder{T}"/>
        /// </summary>
        /// <param name="testBase"></param>
        /// <param name="named"></param>
        public DictionaryStoredStateBuilder(TestBase testBase, string named) : base(testBase)
        {
            Named = named;
        }

        /// <summary>
        /// Set's the specified named value to the container. 
        /// </summary>
        /// <param name="valueToSet"></param>
        /// <returns></returns>
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