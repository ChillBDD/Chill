using System;


namespace TestStack
{
    public abstract class TestFor<TSubject, TResult> : GivenWhenThen<TResult>
        where TSubject : class
    {
        protected TestFor(bool suppressInitialization = false) : base(suppressInitialization)
        {
        }

        private Func<IAutoMockingContainer, TSubject> forSubjectFunc;
        protected TSubject Subject;

        protected override void BeforeWhen()
        {
            if (this.Subject == null)
            {
                this.Subject = forSubjectFunc != null ? forSubjectFunc(Container): ForSubject();
            }
        }

        protected void ForSubject(Func<IAutoMockingContainer, TSubject> subjectFactory)
        {
            forSubjectFunc = subjectFactory;
        }


        protected virtual TSubject ForSubject()
        {
            return Container.Get<TSubject>();
        }
    }

    public abstract class TestFor<TSubject> : GivenWhenThen
        where TSubject : class
    {
        protected TestFor(bool suppressInitialization = false) : base(suppressInitialization)
        {
        }

        private Func<IAutoMockingContainer, TSubject> forSubjectFunc;
        protected TSubject Subject;

        protected override void BeforeWhen()
        {
            if (this.Subject == null)
            {
                this.Subject = forSubjectFunc != null ? forSubjectFunc(Container) : ForSubject();
            }
        }

        protected virtual TSubject ForSubject()
        {
            return Container.Get<TSubject>();
        }

    }
}