using System;
using System.Threading.Tasks;

namespace Chill
{
    public abstract class GivenSubject<TSubject, TResult> : TestFor<TSubject>
        where TSubject : class
    {
        private Func<Task<TResult>> whenAction;
        private TResult _result;

        protected TResult Result
        {
            get { return _result; }
        }

        public Func<Task<TResult>> WhenAction
        {
            get { return whenAction; }
        }

        protected void When(Func<Task<TResult>> whenFunc)
        {
            EnsureSubject();
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            whenAction = whenFunc;
            TriggerTest(async () => _result = await whenAction());
        }

        protected void When(Func<TResult> whenFunc)
        {
            When(() => Task.Factory.StartNew(whenFunc));

        }

        public void Given(Action a)
        {
            EnsureContainer();
            a();
        }

    }

    public abstract class GivenSubject<TSubject> : TestFor<TSubject> where TSubject : class
    {
        private Func<Task> _whenAction;

        public Func<Task> WhenAction
        {
            get { return _whenAction; }
        }

        public void When(Func<Task> whenActionASync)
        {
            EnsureContainer();
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            _whenAction = whenActionASync;
            TriggerTest(_whenAction);

        }
        public void When(Action whenAction)
        {
            When(() => Task.Factory.StartNew(whenAction));
        }

        public void Given(Action a)
        {
            EnsureContainer();
            a();
        }

    }
}