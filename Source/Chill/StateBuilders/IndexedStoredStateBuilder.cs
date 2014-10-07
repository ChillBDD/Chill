using System.Collections.Generic;

namespace Chill.StateBuilders
{
    public class IndexedStoredStateBuilder<T> : StoreStateBuilder<T> where T : class
    {
        public int Index { get; private set; }

        public IndexedStoredStateBuilder(TestBase testBase, int index) : base(testBase)
        {
            Index = index;
        }

        public override TestBase To(T valueToSet)
        {
            var list = TestBase.Container.Get<List<T>>();

            if (list == null)
            {
                list = new List<T>();
            }

            while (list.Count < this.Index)
            {
                list.Add(default(T));
            }

            list.Add(valueToSet);
            TestBase.Container.Set(list);


            return TestBase;
        }
    }
}