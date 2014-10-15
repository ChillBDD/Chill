
internal class AutofacChillContainer : IChillContainer
{
    private IContainer _container;
    private ContainerBuilder _containerBuilder;

    public AutofacChillContainer()
        : this(new ContainerBuilder())
    {
    }
    public AutofacChillContainer(IContainer container)
    {
        _container = container;
    }

    public AutofacChillContainer(ContainerBuilder containerBuilder)
    {
        _containerBuilder = containerBuilder;
    }

    protected IContainer Container
    {
        get
        {
            if (_container == null)
                _container = _containerBuilder.Build();
            return _container;
        }
    }

    public void Dispose()
    {
        Container.Dispose();
    }

    public T Get<T>() where T : class
    {
        return Container.Resolve<T>();
    }

    public T Set<T>(T valueToSet) where T : class
    {
        Container.ComponentRegistry.Register(RegistrationBuilder.ForDelegate((c, p) => valueToSet)
            .InstancePerLifetimeScope().CreateRegistration()
        );
        return Get<T>();
    }
}
