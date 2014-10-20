using System;
using System.Threading.Tasks;

namespace Chill
{
    public abstract class GivenWhenThen<TResult> : TestBase
    {
        private Func<Task<TResult>> whenAction;
        private TResult result;

        protected TResult Result
        {
            get
            {
                return result;
            }
        }

        public Func<Task<TResult>> WhenAction
        {
            get { return whenAction; }
            set
            {
                EnsureContainer(); 
                whenAction = value;
            }
        }

        protected void When(Func<Task<TResult>> whenFunc, bool? deferedExecution = null)
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
            TriggerTest(async () => result = await whenAction(), expectExceptions);
        }

        protected void When(Func<TResult> whenFunc, bool? deferedExecution = null)
        {
            When(() => Task.Factory.StartNew(whenFunc), deferedExecution);

        }

        public void Given(Action a)
        {
            EnsureContainer();
            a();
        }

    }

    public abstract class GivenWhenThen : TestBase
    {
        private Func<Task> whenAction;

        public Func<Task> WhenAction
        {
            get { return whenAction; }
            set
            {
                EnsureContainer();
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
            if (!this.DefferedExecution)
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