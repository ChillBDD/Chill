using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chill
{
    internal static class TaskFromResult
    {
        public static Task<TResult> Create<TResult>(TResult result)
        {
#if NET45
            return Task.FromResult(result);
#else
            var taskCompletionSource = new TaskCompletionSource<TResult>();
            taskCompletionSource.SetResult(result);
            return taskCompletionSource.Task;
#endif
        }
    }

    public static class AsyncTestExtensions
    {
        public static void Given(this IAsyncTestContext context, Action given)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            context.Given(() =>
            {
                given();
                return TaskFromResult.Create(false);
            });
        }

        public static void Given<TSubject>(
            this IAsyncTestContextWithSubject<TSubject> context,
            Func<Func<TSubject>, Task<TSubject>> given)
            where TSubject : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            context.Given(async oldSubjectFactory =>
            {
                TSubject subject = await given(oldSubjectFactory);
                return () => subject;
            });
        }

        public static void Given<TSubject>(
            this IAsyncTestContextWithSubject<TSubject> context,
            Func<Func<TSubject>, Task> given)
            where TSubject : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            context.Given(async oldSubjectFactory =>
            {
                await given(oldSubjectFactory);
                return oldSubjectFactory;
            });
        }

        public static void Given<TSubject>(
            this IAsyncTestContextWithSubject<TSubject> context,
            Func<Task<Func<TSubject>>> given)
            where TSubject : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            context.Given(_ => given());
        }

        public static void Given<TSubject>(
            this IAsyncTestContextWithSubject<TSubject> context,
            Func<Task<TSubject>> given)
            where TSubject : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            context.Given(async _ =>
            {
                TSubject subject = await given();
                return () => subject;
            });
        }

        public static void Given<TSubject>(
            this IAsyncTestContextWithSubject<TSubject> context,
            Func<Task> given)
            where TSubject : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            context.Given(async oldSubjectFactory =>
            {
                await given();
                return oldSubjectFactory;
            });
        }

        public static void Given<TSubject>(
            this IAsyncTestContextWithSubject<TSubject> context,
            Func<Func<TSubject>, Func<TSubject>> given)
            where TSubject : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            context.Given(oldSubjectFactory => TaskFromResult.Create(given(oldSubjectFactory)));
        }

        public static void Given<TSubject>(
            this IAsyncTestContextWithSubject<TSubject> context,
            Func<Func<TSubject>, TSubject> given)
            where TSubject : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            context.Given(oldSubjectFactory =>
            {
                TSubject subject = given(oldSubjectFactory);
                return TaskFromResult.Create<Func<TSubject>>(() => subject);
            });
        }

        public static void Given<TSubject>(
            this IAsyncTestContextWithSubject<TSubject> context,
            Action<Func<TSubject>> given)
            where TSubject : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            context.Given(oldSubjectFactory =>
            {
                given(oldSubjectFactory);
                return TaskFromResult.Create(oldSubjectFactory);
            });
        }

        public static void Given<TSubject>(
            this IAsyncTestContextWithSubject<TSubject> context,
            Func<Func<TSubject>> given)
            where TSubject : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            context.Given(_ => TaskFromResult.Create(given()));
        }

        public static void Given<TSubject>(
            this IAsyncTestContextWithSubject<TSubject> context,
            Func<TSubject> given)
            where TSubject : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            context.Given(_ =>
            {
                TSubject subject = given();
                return TaskFromResult.Create<Func<TSubject>>(() => subject);
            });
        }

        public static void Given<TSubject>(
            this IAsyncTestContextWithSubject<TSubject> context,
            Action given)
            where TSubject : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            context.Given(oldSubjectFactory =>
            {
                given();
                return TaskFromResult.Create(oldSubjectFactory);
            });
        }

        public static void When(this IAsyncTestContextWithoutResult context, Action when)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (when == null)
            {
                throw new ArgumentNullException(nameof(when));
            }

            context.When(() =>
            {
                when();
                return TaskFromResult.Create(false);
            });
        }

        public static void When<TResult>(this IAsyncTestContext<TResult> context, Func<TResult> when)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (when == null)
            {
                throw new ArgumentNullException(nameof(when));
            }

            context.When(() => TaskFromResult.Create(when()));
        }
    }

    public interface IAsyncTestContext
    {
        void Given(Func<Task> given);
    }

    public interface IAsyncTestContextWithoutResult
    {
        void When(Func<Task> when);
    }

    public interface IAsyncTestContext<TResult> : IAsyncTestContext
    {
        void When(Func<Task<TResult>> when);
    }

    public interface IAsyncTestContextWithSubject<TSubject> : IAsyncTestContext
        where TSubject : class
    {
        void Given(Func<Func<TSubject>, Task<Func<TSubject>>> given);
    }

    public interface IAsyncTestContextWithSubject<TSubject, TResult> :
        IAsyncTestContextWithSubject<TSubject>,
        IAsyncTestContext<TResult>
        where TSubject : class
    {
    }

    /// <summary>
    /// Allows definition and execution of <c>Given</c> and <c>When</c> in really async tests
    /// that follow the BDD style GivenWhenThen approach.
    /// For tests that have no subject and no result.
    /// </summary>
    public sealed class AsyncTestContext : IAsyncTestContext, IAsyncTestContextWithoutResult
    {
        private readonly List<Func<Task>> givens = new List<Func<Task>>();
        private Func<Task> when;
        private bool areGivensExecuted;
        private bool isWhenExecuted;

        public async Task ExecuteGiven()
        {
            if (areGivensExecuted)
            {
                throw new InvalidOperationException("Givens have already been executed.");
            }

            areGivensExecuted = true;

            foreach (Func<Task> given in givens)
            {
                Task task = given();

                if (task == null)
                {
                    throw new InvalidOperationException("A Given delegate returned null task.");
                }

                await task;
            }
        }

        public async Task ExecuteWhen()
        {
            if (when == null)
            {
                throw new InvalidOperationException("Each test class must have a When method call.");
            }

            if (!areGivensExecuted)
            {
                await ExecuteGiven();
            }

            if (isWhenExecuted)
            {
                throw new InvalidOperationException("When has already been executed.");
            }

            isWhenExecuted = true;

            Task task = when();

            if (task == null)
            {
                throw new InvalidOperationException("The When delegate returned null task.");
            }

            await task;
        }

        public void Given(Func<Task> given)
        {
            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            givens.Add(given);
        }

        public void When(Func<Task> when)
        {
            if (when == null)
            {
                throw new ArgumentNullException(nameof(when));
            }

            if (this.when != null)
            {
                throw new InvalidOperationException("Each test class can have only one When method call.");
            }

            this.when = when;
        }
    }

    /// <summary>
    /// Allows definition and execution of <c>Given</c> and <c>When</c> in really async tests
    /// that follow the BDD style GivenWhenThen approach.
    /// For tests that have result but no subject.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public sealed class AsyncTestContext<TResult> : IAsyncTestContext<TResult>
    {
        private readonly List<Func<Task>> givens = new List<Func<Task>>();
        private Func<Task<TResult>> when;
        private bool areGivensExecuted;
        private bool isWhenExecuted;

        public async Task ExecuteGiven()
        {
            if (areGivensExecuted)
            {
                throw new InvalidOperationException("Givens have already been executed.");
            }

            areGivensExecuted = true;

            foreach (Func<Task> given in givens)
            {
                Task task = given();

                if (task == null)
                {
                    throw new InvalidOperationException("A Given delegate returned null task.");
                }

                await task;
            }
        }

        public async Task<TResult> ExecuteWhen()
        {
            if (when == null)
            {
                throw new InvalidOperationException("Each test class must have a When method call.");
            }

            if (!areGivensExecuted)
            {
                await ExecuteGiven();
            }

            if (isWhenExecuted)
            {
                throw new InvalidOperationException("When has already been executed.");
            }

            isWhenExecuted = true;

            Task<TResult> task = when();

            if (task == null)
            {
                throw new InvalidOperationException("The When delegate returned null task.");
            }

            return await task;
        }

        public void Given(Func<Task> given)
        {
            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            givens.Add(given);
        }

        public void When(Func<Task<TResult>> when)
        {
            if (when == null)
            {
                throw new ArgumentNullException(nameof(when));
            }

            if (this.when != null)
            {
                throw new InvalidOperationException("Each test class can have only one When method call.");
            }

            this.when = when;
        }
    }

    /// <summary>
    /// Allows definition and execution of <c>Given</c> and <c>When</c> in really async tests
    /// that follow the BDD style GivenWhenThen approach.
    /// For tests that have subject but no result.
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    public sealed class AsyncTestContextWithSubject<TSubject> :
        IAsyncTestContextWithSubject<TSubject>,
        IAsyncTestContextWithoutResult
        where TSubject : class
    {
        private readonly TestBase test;

        private readonly List<Func<Func<TSubject>, Task<Func<TSubject>>>> givens =
            new List<Func<Func<TSubject>, Task<Func<TSubject>>>>();

        private TSubject subject;
        private bool areGivensExecuted;
        private Func<TSubject, Task> when;
        private bool isWhenExecuted;

        public AsyncTestContextWithSubject(TestBase test)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            this.test = test;
        }

        public async Task<TSubject> ExecuteGiven()
        {
            if (areGivensExecuted)
            {
                throw new InvalidOperationException("Givens have already been executed.");
            }

            areGivensExecuted = true;

            Func<TSubject> currentSubjectFactory = () =>
            {
                if (!test.Container.IsRegistered<TSubject>())
                {
                    test.Container.RegisterType<TSubject>();
                }

                return test.Container.Get<TSubject>();
            };

            foreach (Func<Func<TSubject>, Task<Func<TSubject>>> given in givens)
            {
                Task<Func<TSubject>> task = given(currentSubjectFactory);

                if (task == null)
                {
                    throw new InvalidOperationException("A Given delegate returned null task.");
                }

                currentSubjectFactory = await task;

                if (currentSubjectFactory == null)
                {
                    throw new InvalidOperationException("A Given delegate produced null subject factory delegate.");
                }
            }

            subject = currentSubjectFactory();

            if (subject == null)
            {
                throw new InvalidOperationException("A Given delegate produced null subject.");
            }

            if (!test.Container.IsRegistered<TSubject>())
            {
                test.Container.Set(subject);
            }

            return subject;
        }

        public async Task ExecuteWhen()
        {
            if (when == null)
            {
                throw new InvalidOperationException("Each test class must have a When method call.");
            }

            if (!areGivensExecuted)
            {
                await ExecuteGiven();
            }

            if (isWhenExecuted)
            {
                throw new InvalidOperationException("When has already been executed.");
            }

            isWhenExecuted = true;

            Task task = when(subject);

            if (task == null)
            {
                throw new InvalidOperationException("The When delegate returned null task.");
            }

            await task;
        }

        public void Given(Func<Func<TSubject>, Task<Func<TSubject>>> given)
        {
            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            givens.Add(given);
        }

        public void Given(Func<Task> given)
        {
            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            givens.Add(async subjectFactory =>
            {
                await given();
                return subjectFactory;
            });
        }

        public void When(Func<TSubject, Task> when)
        {
            if (when == null)
            {
                throw new ArgumentNullException(nameof(when));
            }

            if (this.when != null)
            {
                throw new InvalidOperationException("Each test class can have only one When method call.");
            }

            this.when = when;
        }

        public void When(Func<Task> when)
        {
            if (when == null)
            {
                throw new ArgumentNullException(nameof(when));
            }

            When(_ => when());
        }

        public void When(Action<TSubject> when)
        {
            if (when == null)
            {
                throw new ArgumentNullException(nameof(when));
            }

            When(subject =>
            {
                when(subject);
                return TaskFromResult.Create(false);
            });
        }

        public void When(Action when)
        {
            if (when == null)
            {
                throw new ArgumentNullException(nameof(when));
            }

            When(_ =>
            {
                when();
                return TaskFromResult.Create(false);
            });
        }
    }

    /// <summary>
    /// Allows definition and execution of <c>Given</c> and <c>When</c> in really async tests
    /// that follow the BDD style GivenWhenThen approach.
    /// For tests that have result but no subject.
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public sealed class AsyncTestContextWithSubject<TSubject, TResult> :
        IAsyncTestContextWithSubject<TSubject>,
        IAsyncTestContext<TResult>
        where TSubject : class
    {
        private readonly TestBase test;

        private readonly List<Func<Func<TSubject>, Task<Func<TSubject>>>> givens =
            new List<Func<Func<TSubject>, Task<Func<TSubject>>>>();

        private TSubject subject;
        private bool areGivensExecuted;
        private Func<TSubject, Task<TResult>> when;
        private bool isWhenExecuted;

        public AsyncTestContextWithSubject(TestBase test)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            this.test = test;
        }

        public async Task<TSubject> ExecuteGiven()
        {
            if (areGivensExecuted)
            {
                throw new InvalidOperationException("Givens have already been executed.");
            }

            areGivensExecuted = true;

            Func<TSubject> currentSubjectFactory = () =>
            {
                if (!test.Container.IsRegistered<TSubject>())
                {
                    test.Container.RegisterType<TSubject>();
                }

                return test.Container.Get<TSubject>();
            };

            foreach (Func<Func<TSubject>, Task<Func<TSubject>>> given in givens)
            {
                Task<Func<TSubject>> task = given(currentSubjectFactory);

                if (task == null)
                {
                    throw new InvalidOperationException("A Given delegate returned null task.");
                }

                currentSubjectFactory = await task;

                if (currentSubjectFactory == null)
                {
                    throw new InvalidOperationException("A Given delegate produced null subject factory delegate.");
                }
            }

            subject = currentSubjectFactory();

            if (subject == null)
            {
                throw new InvalidOperationException("A Given delegate produced null subject.");
            }

            if (!test.Container.IsRegistered<TSubject>())
            {
                test.Container.Set(subject);
            }

            return subject;
        }

        public async Task<TResult> ExecuteWhen()
        {
            if (when == null)
            {
                throw new InvalidOperationException("Each test class must have a When method call.");
            }

            if (!areGivensExecuted)
            {
                await ExecuteGiven();
            }

            if (isWhenExecuted)
            {
                throw new InvalidOperationException("When has already been executed.");
            }

            isWhenExecuted = true;

            Task<TResult> task = when(subject);

            if (task == null)
            {
                throw new InvalidOperationException("The When delegate returned null task.");
            }

            return await task;
        }

        public void Given(Func<Func<TSubject>, Task<Func<TSubject>>> given)
        {
            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            givens.Add(given);
        }

        public void Given(Func<Task> given)
        {
            if (given == null)
            {
                throw new ArgumentNullException(nameof(given));
            }

            givens.Add(async subjectFactory =>
            {
                await given();
                return subjectFactory;
            });
        }

        public void When(Func<TSubject, Task<TResult>> when)
        {
            if (when == null)
            {
                throw new ArgumentNullException(nameof(when));
            }

            if (this.when != null)
            {
                throw new InvalidOperationException("Each test class can have only one When method call.");
            }

            this.when = when;
        }

        public void When(Func<Task<TResult>> when)
        {
            if (when == null)
            {
                throw new ArgumentNullException(nameof(when));
            }

            When(_ => when());
        }

        public void When(Func<TSubject, TResult> when)
        {
            if (when == null)
            {
                throw new ArgumentNullException(nameof(when));
            }

            When(subject => TaskFromResult.Create(when(subject)));
        }

        public void When(Func<TResult> when)
        {
            if (when == null)
            {
                throw new ArgumentNullException(nameof(when));
            }

            When(_ => TaskFromResult.Create(when()));
        }
    }
}