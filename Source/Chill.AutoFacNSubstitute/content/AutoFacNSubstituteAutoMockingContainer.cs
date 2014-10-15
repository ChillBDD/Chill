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

    public T Get<T>(string key = null) where T : class
    {
        if (key == null)
        {
            return _substitute.Container.Resolve<T>();
        }
        else
        {
            return _substitute.Container.ResolveKeyed<T>(key);
        }
    }

    public T Set<T>(T valueToSet, string key) where T : class
    {
        if (key == null)
        {
            return _substitute.Provide<T>(valueToSet);
        }
        else
        {
            return _substitute.Provide<T>(valueToSet, key);
        }
    }
}


