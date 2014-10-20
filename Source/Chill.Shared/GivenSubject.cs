using System;
using System.Threading.Tasks;

namespace Chill
{
    public abstract class GivenSubject<TSubject, TResult> : TestFor<TSubject>
        where TSubject : class
    {
        private Func<Task<TResult>> whenAction;
        private TResult result;

        protected TResult Result
        {
            get { return result; }
        }

        public Func<Task<TResult>> WhenAction
        {
            get { return whenAction; }
            set
            {
                EnsureSubject(); 
                whenAction = value;
            }
        }

        protected void When(Func<Task<TResult>> whenFunc, bool? deferedExecution = null)
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

        protected void When(Func<TResult> whenFunc, bool? deferedExecution = null)
        {
            When(() => Task.Factory.StartNew(whenFunc), deferedExecution);

        }

        internal override void TriggerTest(bool expectExceptions)
        {
            TriggerTest(async () => result = await whenAction(), expectExceptions);
        }

        public void Given(Action a)
        {
            EnsureContainer();
            a();
        }

    }

    public abstract class GivenSubject<TSubject> : TestFor<TSubject> where TSubject : class
    {
        private Func<Task> whenAction;

        public Func<Task> WhenAction
        {
            get { return whenAction; }
            set
            {
                EnsureSubject(); 
                whenAction = value;
            }
        }

        public void When(Func<Task> whenActionASync, bool? deferedExecution = null)
        {
            DefferedExecution = deferedExecution ?? DefferedExecution;
            EnsureContainer();
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            whenAction = whenActionASync;
            if (!DefferedExecution)
            {
                EnsureTestTriggered(false);
            }

        }
        public void When(Action whenAction, bool? deferedExecution = null)
        {
            When(() => Task.Factory.StartNew(whenAction), deferedExecution);
        }

        internal override void TriggerTest(bool expectExceptions)
        {
            TriggerTest(async () => await whenAction(), expectExceptions);
        }

        public void Given(Action a)
        {
            EnsureContainer();
            a();
        }

    }
}