using System;
using System.Security.Cryptography.X509Certificates;

namespace TestStack
{

    public abstract class GivenWhenThen<TResult> : TestBase
    {
        protected GivenWhenThen(bool suppressInitialization = false) : base()
        {
        }


        protected TResult Result
        {
            get
            {
                return _result;
            }
        }

        public Func<TResult> WhenAction
        {
            get { return whenAction; }
        }

        protected virtual void BeforeWhen()
        {
            
        }

        protected virtual void TriggerWhen()
        {
            EnsureContainer();
            BeforeWhen();
            _result = whenAction();
        }

        protected void When(Func<TResult> whenFunc)
        {
            EnsureContainer();
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            whenAction = whenFunc;
            TriggerWhen();
        }

        private Func<TResult> whenAction;
        private TResult _result;
    }

    public abstract class GivenWhenThen : TestBase
    {
        protected GivenWhenThen(bool suppressInitialization = false) : base()
        {
        }

        private Action _whenAction;
        private bool _callWhenAction;

        public Action WhenAction
        {
            get { return _whenAction; }
            set { _whenAction = value; }
        }

        protected override void TriggerWhen()
        {
            EnsureContainer();
            BeforeWhen();
            WhenAction();
        }
        protected virtual void BeforeWhen()
        {

        }


        protected void When(Action whenAction)
        {
            _callWhenAction = true;
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            _whenAction = whenAction;
        }
    }
}