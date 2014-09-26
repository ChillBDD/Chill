using System;

using AutofacContrib.NSubstitute;

namespace TestStack
{
    public abstract class TestFor<TSubject, TResult> : GivenWhenThen<TResult>
        where TSubject : class
    {
        private Func<AutoSubstitute, TSubject> forSubjectFunc;
        protected TSubject Subject;

        protected override void AfterGiven()
        {
            if (this.Subject == null)
            {
                this.Subject = forSubjectFunc == null ? forSubjectFunc(Container): ForSubject();
            }
        }

        protected void ForSubject(Func<AutoSubstitute, TSubject> subjectFactory)
        {
            forSubjectFunc = subjectFactory;
        }


        protected virtual TSubject ForSubject()
        {
            return Container.Resolve<TSubject>();
        }
    }

    public abstract class TestFor<TSubject> : GivenWhenThen
        where TSubject : class
    {
        private Func<AutoSubstitute, TSubject> forSubjectFunc;
        protected TSubject Subject;

        protected override void AfterGiven()
        {
            if (this.Subject == null)
            {
                this.Subject = forSubjectFunc == null ? forSubjectFunc(Container) : ForSubject();
            }
        }

        protected virtual TSubject ForSubject()
        {
            return Container.Resolve<TSubject>();
        }

    }
}