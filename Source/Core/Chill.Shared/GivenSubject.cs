using System;
using System.Threading.Tasks;

namespace Chill
{
    /// <summary>
    /// BDD style test base class for tests that have a fixed subject and a fixed result. 
    /// The TSubject type defines the type of the subject, and the TResult defines the type of the result. 
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject</typeparam>esu
    /// <typeparam name="TResult">The type of the result</typeparam>
    public abstract class GivenSubject<TSubject, TResult> : TestFor<TSubject>
        where TSubject : class
    {
        private Func<TResult> whenAction;
        private TResult result;

        /// <summary>
        /// The result of the tests 
        /// </summary>
        protected TResult Result
        {
            get { return result; }
        }

        /// <summary>
        /// The action that triggers the test. Typically used in combination with deffered execution. 
        /// </summary>
        public Func<TResult> WhenAction
        {
            get { return whenAction; }
            set
            {
                EnsureSubject(); 
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
            EnsureSubject();
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            whenAction = whenFunc;
            if (!DefferedExecution)
            {
                EnsureTestTriggered(false);
            }
        }
        /// <summary>
        /// Records the asynchronous action that will trigger the actual test
        /// </summary>
        /// <param name="whenFunc"></param>
        /// <param name="deferedExecution">Should the test be executed immediately or be deffered?</param>
        protected void When(Func<Task<TResult>> whenFunc, bool? deferedExecution = null)
        {
            When(() => whenFunc().Result, deferedExecution);

        }

        internal override void TriggerTest(bool expectExceptions)
        {
            TriggerTest(() => result = whenAction(), expectExceptions);
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
    /// BDD style test base class for tests that have a fixed subject. 
    /// The TSubject type defines the type of the subject. 
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject</typeparam>
    public abstract class GivenSubject<TSubject> : TestFor<TSubject> where TSubject : class
    {
        private Action whenAction;

        /// <summary>
        /// The action that triggers the test. Typically used in combination with deffered execution. 
        /// </summary>
        public Action WhenAction
        {
            get { return whenAction; }
            set
            {
                EnsureSubject(); 
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
            if (!DefferedExecution)
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
            When(() => whenActionAsync().Wait(), deferedExecution);
        }

        internal override void TriggerTest(bool expectExceptions)
        {
            TriggerTest(whenAction, expectExceptions);
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