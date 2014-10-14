using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.ResolveAnything;
using AutofacContrib.NSubstitute;
using Chill;
using NSubstitute;

internal class AutofacNSubstituteChillContainer : IChillContainer
{
    private IContainer _container;
    private AutoSubstitute _substitute;


    public AutofacNSubstituteChillContainer()
    {
        _substitute = new AutoSubstitute(ConfigureContainer);
    }

    private void ConfigureContainer(ContainerBuilder obj)
    {
    }

    public void Dispose()
    {
        _substitute.Dispose();
    }

    public T Get<T>() where T : class
    {
        return _substitute.Resolve<T>();
    }

    public T Set<T>(T valueToSet) where T : class
    {
        return _substitute.Provide<T>(valueToSet);
    }
}


internal class AutofacChillContainer : IChillContainer
{
    private IContainer _container;
    private ContainerBuilder _containerBuilder;

    public AutofacChillContainer() : this(new ContainerBuilder())
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

