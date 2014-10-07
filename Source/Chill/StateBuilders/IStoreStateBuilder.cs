namespace Chill.StateBuilders
{
    public interface IStoreStateBuilder<T>
    {
        TestBase TestBase { get; set; }

        TestBase To(T valueToSet);

    }
}