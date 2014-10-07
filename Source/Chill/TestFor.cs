using System;

namespace Chill
{
    public abstract class TestFor<TSubject>  : TestBase
        where TSubject : class
    {
        private Func<IAutoMockingContainer, TSubject> forSubjectFunc;
        private TSubject subject;

        protected TSubject Subject
        {
            get
            {
                EnsureSubject();

                return subject;
            }
        }

        protected void EnsureSubject()
        {
            EnsureContainer();
            if (this.subject == null)
            {
                this.subject = forSubjectFunc != null ? forSubjectFunc(Container) : ForSubject();
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
}