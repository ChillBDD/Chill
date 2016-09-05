using System;
using System.Threading.Tasks;

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
        /// The action that triggers the actual test. This can be used in combination with deffered execution and fluent assertions 
        /// to detect exceptions, if you don't wnat to use the <see cref="TestBase.CaughtException"/>
        /// </summary>
        public Func<TResult> WhenAction
        {
            get { return whenAction; }
            set
            {
                EnsureContainer(); 
                whenAction = value;
            }
        }

        /// <summary>
        /// Records the action that will trigger the actual test
        /// </summary>
        /// <param name="whenFunc"></param>
        /// <param name="deferedExecution">Should the test be executed immediately or be deffered?</param>
        protected void When(Func<TResult> whenFunc, bool? deferedExecution = null)
        {
            DefferedExecution = deferedExecution ?? DefferedExecution;
            EnsureContainer();
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            whenAction = whenFunc;
            if (!this.DefferedExecution)
            {
                EnsureTestTriggered(false);
            }

        }

        internal override void TriggerTest(bool expectExceptions)
        {
            TriggerTest(() => result = whenAction(), expectExceptions);
        }

        /// <summary>
        /// Records the asynchronous action that will trigger the actual test
        /// </summary>
        /// <param name="whenFunc"></param>
        /// <param name="deferedExecution">Should the test be executed immediately or be deffered?</param>
        protected void When(Func<Task<TResult>> whenFunc, bool? deferedExecution = null)
        {
#if NET45
            When(() => Task.Run(whenFunc).Result, deferedExecution);
#else
            When(() => Task.Factory.StartNew(whenFunc).Result, deferedExecution);
#endif
        }

        /// <summary>
        /// Records a precondition
        /// </summary>
        /// <param name="a"></param>
        public void Given(Action a)
        {
            EnsureContainer();
            a();
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
        /// The action that triggers the actual test. This can be used in combination with deffered execution and fluent assertions 
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
        /// Records the action that will trigger the actual test
        /// </summary>
        /// <param name="whenAction"></param>
        /// <param name="deferedExecution">Should the test be executed immediately or be deffered?</param>
        public void When(Action whenAction, bool? deferedExecution = null)
        {
            DefferedExecution = deferedExecution ?? DefferedExecution;
            EnsureContainer();
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            this.whenAction = whenAction;
            if (!this.DefferedExecution)
            {
                EnsureTestTriggered(false);
            }

        }

        /// <summary>
        /// Records the asynchronous action that will trigger the actual test
        /// </summary>
        /// <param name="whenActionAsync"></param>
        /// <param name="deferedExecution">Should the test be executed immediately or be deffered?</param>
        public void When(Func<Task> whenActionAsync, bool? deferedExecution = null)
        {
#if NET45
            When(() => Task.Run(whenActionAsync).Wait(), deferedExecution);
#else
            When(() => whenActionAsync().Wait(), deferedExecution);
#endif
        }

        internal override void TriggerTest(bool expectExceptions)
        {
            TriggerTest(() => whenAction(), expectExceptions);
        }

        /// <summary>
        /// Records a precondition
        /// </summary>
        /// <param name="a"></param>
        public void Given(Action a)
        {
            EnsureContainer();
            a();
        }
    }
}