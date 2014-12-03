using System;
using System.Collections.Generic;
using System.Linq;
using Chill.Unity;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using NSubstitute;

namespace Chill.UnityNSubstitute
{
    /// <summary>
    /// Automocking container that uses NSubstitute to create mocks and Autofac as the container. 
    /// </summary>
    internal class UnityNSubstituteChillContainer : UnityChillContainer
    {

        public UnityNSubstituteChillContainer()
        {
            this.Container.AddNewExtension<UnityAutoMockingContainerExtension>();
        }


        private class UnityAutoMockingContainerExtension : UnityContainerExtension
        {
            protected override void Initialize()
            {
                var strategy = new AutoMockingBuilderStrategy(this.Container);
                Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
            }
        }

        private class AutoMockingBuilderStrategy : BuilderStrategy
        {
            private readonly IUnityContainer _container;

            public AutoMockingBuilderStrategy(IUnityContainer container)
            {
                _container = container;
            }

            public override void PreBuildUp(IBuilderContext context)
            {
                var key = context.OriginalBuildKey;
                if (key.Type.IsInterface && !_container.IsRegistered(key.Type, key.Name))
                {
                    context.Existing = CreateDynamicMock(key.Type);
                    this._container.RegisterInstance(key.Type, key.Name, context.Existing);
                }
            }

            private static object CreateDynamicMock(Type type)
            {
                return Substitute.For(new Type[] {type}, null);
            }
        }
    }
}