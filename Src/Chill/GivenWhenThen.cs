using System;
using System.Threading.Tasks;
using Chill.Common;

namespace Chill
{
    /// <summary>
    /// Baseclass for tests that follow the BDD style GivenWhenThen approach, but do not have 
    /// a fixed subject. Use the TResult parameter to specify what the result type 
    /// will be. 
    /// </summary>
    /// <typeparam name="TResult">The type of the result that you expect from your tests</typeparam>
    public abstract class GivenWhenThen<TResult> : TestBase
    {
        private Func<TResult> whenAction;
        private TResult result;

        /// <summary>
        /// The result of your test. 
        /// </summary>
        protected TResult Result
        {
            get
            {
                return result;
            }
        }

        /// <summary>
        /// The action that triggers the actual test. This can be used in combination with deferred execution and fluent assertions 
        /// to detect exceptions, if you don't wnat to use the <see cref="TestBase.CaughtException"/>
        /// </summary>
        public Func<TResult> WhenAction
        {
            get
            {
                return () =>
                {
                    result = whenAction();
                    return result;
                };
            }
            set
            {
                EnsureContainer(); 
                whenAction = value;
            }
        }

        /// <summary>
        /// Records the asynchronous action that will trigger the actual test, when later executing the <see cref="WhenAction"/>
        /// </summary>
        protected void WhenLater(Func<TResult> whenFunc)
        {
            When(whenFunc, deferredExecution: true);
        }

        internal override void TriggerTest(bool expectExceptions)
        {
            TriggerTest(() => result = whenAction(), expectExceptions);
        }

        /// <summary>
        /// Records the asynchronous action that will trigger the actual test, when later executing the <see cref="WhenAction"/>
        /// </summary>
        protected void WhenLater(Func<Task<TResult>> whenFunc)
        {
            When(whenFunc, deferredExecution: true);
        }

        /// <summary>
        /// Records the asynchronous action that will trigger the actual test
        /// </summary>
        /// <param name="whenFunc"></param>
        /// <param name="deferredExecution">Should the test be executed immediately or be deferred?</param>
        protected void When(Func<Task<TResult>> whenFunc, bool? deferredExecution = null)
        {
            When(() => whenFunc().GetAwaiter().GetResult(), deferredExecution);
        }

        /// <summary>
        /// Records the action that will trigger the actual test
        /// </summary>
        /// <param name="whenFunc"></param>
        /// <param name="deferredExecution">Should the test be executed immediately or be deferred?</param>
        protected void When(Func<TResult> whenFunc, bool? deferredExecution = null)
        {
            DeferredExecution = deferredExecution ?? DeferredExecution;
            EnsureContainer();
            if (whenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            
            whenAction = whenFunc.ExecuteInDefaultSynchronizationContext;
            if (!this.DeferredExecution)
            {
                EnsureTestTriggered(false);
            }
        }

        /// <summary>
        /// Records an asynchronous precondition.
        /// </summary>
        public void Given(Func<Task> givenActionAsync)
        {
            Given(() => givenActionAsync().GetAwaiter().GetResult());
        }

        /// <summary>
        /// Records a precondition
        /// </summary>
        /// <param name="action"></param>
        public void Given(Action action)
        {
            EnsureContainer();
            action.ExecuteInDefaultSynchronizationContext();
        }
    }

    /// <summary>
    /// Baseclass for tests that follow the BDD style GivenWhenThen approach, but do not have 
    /// a fixed subject. This class does not use a predefined subject. 
    /// </summary>
    public abstract class GivenWhenThen : TestBase
    {
        private Action whenAction;

        /// <summary>
        /// The action that triggers the actual test. This can be used in combination with deferred execution and fluent assertions 
        /// to detect exceptions, if you don't wnat to use the <see cref="TestBase.CaughtException"/>
        /// </summary>
        public Action WhenAction
        {
            get { return whenAction; }
            set
            {
                EnsureContainer();
                whenAction = value;
            }
        }

        /// <summary>
        /// Records the asynchronous action that will trigger the actual test, when later executing the <see cref="WhenAction"/>
        /// </summary>
        protected void WhenLater(Action whenAction)
        {
            When(whenAction, deferredExecution: true);
        }

        /// <summary>
        /// Records the asynchronous action that will trigger the actual test, when later executing the <see cref="WhenAction"/>
        /// </summary>
        protected void WhenLater(Func<Task> whenActionAsync)
        {
            When(() => whenActionAsync().GetAwaiter().GetResult(), deferredExecution: true);
        }

        /// <summary>
        /// Records the asynchronous action that will trigger the actual test
        /// </summary>
        /// <param name="whenActionAsync"></param>
        /// <param name="deferredExecution">Should the test be executed immediately or be deferred?</param>
        public void When(Func<Task> whenActionAsync, bool? deferredExecution = null)
        {
            When(() => whenActionAsync().GetAwaiter().GetResult(), deferredExecution);
        }

        /// <summary>
        /// Records the action that will trigger the actual test
        /// </summary>
        /// <param name="whenAction"></param>
        /// <param name="deferredExecution">Should the test be executed immediately or be deferred?</param>
        public void When(Action whenAction, bool? deferredExecution = null)
        {
            DeferredExecution = deferredExecution ?? DeferredExecution;
            EnsureContainer();
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            this.whenAction = whenAction.ExecuteInDefaultSynchronizationContext;
            if (!DeferredExecution)
            {
                EnsureTestTriggered(false);
            }

        }

        internal override void TriggerTest(bool expectExceptions)
        {
            TriggerTest(() => whenAction(), expectExceptions);
        }

        /// <summary>
        /// Records an asynchronous precondition.
        /// </summary>
        public void Given(Func<Task> givenActionAsync)
        {
            Given(() => givenActionAsync().GetAwaiter().GetResult());
        }

        /// <summary>
        /// Records a precondition
        /// </summary>
        /// <param name="action"></param>
        public void Given(Action action)
        {
            EnsureContainer();
            action.ExecuteInDefaultSynchronizationContext();
        }
    }
}