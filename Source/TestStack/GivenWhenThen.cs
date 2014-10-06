using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace TestStack
{

    public abstract class GivenWhenThen<TResult> : TestBase
    {
        private Func<Task<TResult>> whenAction;
        private TResult _result;

        protected GivenWhenThen() : base()
        {
        }

        protected TResult Result
        {
            get
            {
                return _result;
            }
        }

        public Func<Task<TResult>> WhenAction
        {
            get { return whenAction; }
        }

        protected virtual void BeforeWhen()
        {
            
        }

        protected override async Task TriggerWhen()
        {
            EnsureContainer();
            BeforeWhen();
            _result = await whenAction();
        }

        protected void When(Func<Task<TResult>> whenFunc)
        {
            EnsureContainer();
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            whenAction = whenFunc;
            TriggerWhen().Wait();
        }
    
        protected void When(Func<TResult> whenFunc)
        {

            When(() => Task.Factory.StartNew(whenFunc));

        }

    }

    public abstract class GivenWhenThen : TestBase
    {
        private Func<Task> _whenAction;

        public Func<Task> WhenAction
        {
            get { return _whenAction; }
            set { _whenAction = value; }
        }

        protected override async Task TriggerWhen()
        {
            EnsureContainer();
            BeforeWhen();
            await WhenAction();
        }
        protected virtual void BeforeWhen()
        {

        }

        protected void When(Func<Task> whenActionASync)
        {
            EnsureContainer();
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            _whenAction = whenActionASync;
            TriggerWhen().Wait();
        }
        protected void When(Action whenAction)
        {
            When(() => Task.Factory.StartNew(whenAction));
        }
    }
}