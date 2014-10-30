using System;
using System.Threading.Tasks;

namespace Chill
{
    public abstract class GivenSubject<TSubject, TResult> : TestFor<TSubject>
        where TSubject : class
    {
        private Func<TResult> whenAction;
        private TResult result;

        protected TResult Result
        {
            get { return result; }
        }

        public Func<TResult> WhenAction
        {
            get { return whenAction; }
            set
            {
                EnsureSubject(); 
                whenAction = value;
            }
        }

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

        protected void When(Func<Task<TResult>> whenFunc, bool? deferedExecution = null)
        {
            When(() => whenFunc().Result, deferedExecution);

        }

        internal override void TriggerTest(bool expectExceptions)
        {
            TriggerTest(() => result = whenAction(), expectExceptions);
        }

        public void Given(Action a)
        {
            EnsureContainer();
            a();
        }

    }

    public abstract class GivenSubject<TSubject> : TestFor<TSubject> where TSubject : class
    {
        private Action whenAction;

        public Action WhenAction
        {
            get { return whenAction; }
            set
            {
                EnsureSubject(); 
                whenAction = value;
            }
        }

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
        public void When(Func<Task> whenActionAsync, bool? deferedExecution = null)
        {
            When(() => whenActionAsync().Wait(), deferedExecution);
        }

        internal override void TriggerTest(bool expectExceptions)
        {
            TriggerTest(whenAction, expectExceptions);
        }

        public void Given(Action a)
        {
            EnsureContainer();
            a();
        }

    }
}