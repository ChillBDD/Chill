using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutofacContrib.NSubstitute;

using NSubstitute;

using Xunit;

namespace TestStack
{
    /// <summary>
    /// Base class for tests that use a similar appraoch. 
    /// </summary>
    public abstract class TestBase : IUseFixture<object>, IDisposable
    {
        protected bool ExpectExceptions;
        protected Exception CaughtException;
        private List<GivenData> _givens = new List<GivenData>();
        protected AutoSubstitute Container;


        public void SetupTestProcess()
        {
            Container = BuildUnityContainer();

            Given();
            AfterGiven();
            try
            {
                TriggerWhen();
            }
            catch (Exception ex)
            {
                CaughtException = ex;
            }
            finally
            {
                if (ExpectExceptions && CaughtException == null)
                    throw new InvalidOperationException("Expected exception but no exception was thrown");
            }
            
        }

        protected virtual void AfterGiven()
        {
            
        }

        protected virtual void TriggerWhen()
        {
            
        }

        public void Given(Action a)
        {
            _givens.Add(new GivenData()
            {
                GivenAction = a
            });
        }

        protected virtual void Given()
        {
            foreach (var given in _givens)
            {
                given.GivenAction();
            }
        }

        protected virtual AutoSubstitute BuildUnityContainer()
        {
            var buildUnityContainer = new AutoSubstitute();
            return buildUnityContainer;
        }


        /// <summary>
        /// Create a substitute and use that from now on. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T The<T>()
            where T : class
        {
            return Container.Resolve<T>();
        }

        protected T GetFromIndex<T>(int index)
        {
            var list = Container.Resolve<List<T>>();
            if (list == null || list.Count >= index)
            {
                return default(T);
            }
            return list[index];
        }

        /// <summary>
        /// Configure the container to use a specific subject from now on. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected T Store<T>(T subject, string key = null)
            where T:class
        {
            Container.Provide<T>(subject);
            return subject;
        }

        /// <summary>
        /// Configure the container to use a specific subject from now on. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected T StoreAtIndex<T>(int index, T subject, string key = null)
            where T : class
        {
            var list = Container.Resolve<List<T>>();
            if (list == null)
            {
                Container.Provide<List<T>>(new List<T>());
            }

            while (list.Count <= index)
            {
                list.Add(null);
            }
            
            list[index] = subject;
            
            return subject;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Container.Dispose();
            }
        }

        public void SetFixture(object data)
        {
            SetupTestProcess();
        }
    }
}
