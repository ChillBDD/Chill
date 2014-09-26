using System;

namespace TestStack
{
    public abstract class GivenWhenThen<TResult> : TestBase
    {
        protected TResult Result
        {
            get
            {
                if (WhenAction == null)
                    throw new InvalidOperationException("When function was never assigned, so result was not set");
                return _result;
            }
        }

        public Func<TResult> WhenAction
        {
            get { return whenAction; }
        }

        protected override void TriggerWhen()
        {
            _result = When();
        }

        protected virtual TResult When()
        {
            if (WhenAction != null)
            {
                return WhenAction();
            }
            return default (TResult);
        }
        protected void When(Func<TResult> whenFunc)
        {
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            whenAction = whenFunc;
        }

        private Func<TResult> whenAction;
        private TResult _result;
    }

    public abstract class GivenWhenThen : TestBase
    {
        private Action _whenAction;
        private bool _callWhenAction;

        public Action WhenAction
        {
            get { return _whenAction; }
            set { _whenAction = value; }
        }

        protected override void TriggerWhen()
        {
            When();
        }

        protected virtual void When()
        {
            if (WhenAction != null && _callWhenAction)
            {
                WhenAction();
            }
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