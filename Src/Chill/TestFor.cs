using System;

namespace Chill
{
    /// <summary>
    /// Base class for tests that set up a Subject Under Test, called <see cref="Subject"/>. Normally, this <see cref="Subject"/> property
    /// is created by the container and any dependencies automatically injected. However,  can influence the creation of the subject by 
    /// calling the <see cref="WithSubject"/> extension method and passing a subjectFactory method. You can also override 
    /// the <see cref="BuildSubject"/> method. 
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject you're testing. </typeparam>
    /// <remarks>
    /// We purposely named this class so badly because we want to avoid a consumer of our package to
    /// accidentally select it when deriving from a class starting with 'G'.
    /// </remarks>
    public abstract class TestFor<TSubject> : TestBase
        where TSubject : class
    {
        private Func<IChillContainer, TSubject> subjectFactory;
        private TSubject subject;

        /// <summary>
        /// The subject under test. Normally, this <see cref="Subject"/> property
        /// is created by the container and any dependencies automatically injected. However,  can influence the creation of the subject by 
        /// calling the <see cref="WithSubject"/> extension method and passing a subjectFactory method. You can also override 
        /// the <see cref="BuildSubject"/> method. 
        /// </summary>
        protected TSubject Subject
        {
            get
            {
                EnsureSubject();

                return subject;
            }
        }

        /// <summary>
        /// Ensures the subject is created
        /// </summary>
        internal void EnsureSubject()
        {
            EnsureContainer();
            if (subject == null)
            {
                Container.RegisterType<TSubject>();
                subject = (subjectFactory != null) ? subjectFactory(Container) : BuildSubject();
            }
        }

        /// <summary>
        /// Call this method to override how the subject is being created, or to augment the created subject. 
        /// </summary>
        /// <param name="subjectFactory">The factory method that will create the subject for you. </param>
        protected void WithSubject(Func<IChillContainer, TSubject> subjectFactory)
        {
            this.subjectFactory = subjectFactory;
        }

        /// <summary>
        /// Method that you can override to create the subject. Note, this method will only get called if the <see cref="WithSubject"/>
        /// method is not called. 
        /// </summary>
        /// <returns></returns>
        protected virtual TSubject BuildSubject()
        {
            return Container.Get<TSubject>();
        }

        protected override void Dispose(bool disposing)
        {
            var disposable = subject as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}