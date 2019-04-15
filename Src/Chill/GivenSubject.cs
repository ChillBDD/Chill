﻿using System;
using System.Threading.Tasks;
using Chill.Common;

namespace Chill
{
    /// <summary>
    /// BDD style test base class for tests that have a fixed subject and a fixed result. 
    /// The TSubject type defines the type of the subject, and the TResult defines the type of the result. 
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    public abstract class GivenSubject<TSubject, TResult> : TestFor<TSubject>
        where TSubject : class
    {
        private Func<TResult> whenAction;
        private TResult result;

        /// <summary>
        /// The result of the tests 
        /// </summary>
        protected TResult Result
        {
            get
            {
                EnsureTestTriggered(false);
                return result;
            }
        }

        /// <summary>
        /// The action that triggers the test. Typically used in combination with deferred execution. 
        /// </summary>
        public Func<TResult> WhenAction
        {
            get { return whenAction; }
            set
            {
                EnsureSubject(); 
                whenAction = value;
            }
        }

        /// <summary>
        /// Records the action that will trigger the actual test, when later executing the <see cref="WhenAction"/>
        /// </summary>
        protected void WhenLater(Func<TResult> whenFunc)
        {
            When(whenFunc, deferredExecution: true);
        }

        /// <summary>
        /// Records the asynchronous action that will trigger the actual test, when later executing the <see cref="WhenAction"/>
        /// </summary>
        protected void WhenLater(Func<Task<TResult>> whenFunc)
        {
            When(whenFunc, deferredExecution: true);
        }

        /// <summary>
        /// Records the asynchronous action that will trigger the actual test
        /// </summary>
        /// <param name="whenFunc"></param>
        /// <param name="deferredExecution">Should the test be executed immediately or be deferred?</param>
        protected void When(Func<Task<TResult>> whenFunc, bool? deferredExecution = null)
        {
            When(() => whenFunc().GetAwaiter().GetResult(), deferredExecution);
        }

        /// <summary>
        /// Records the action that will trigger the actual test
        /// </summary>
        /// <param name="whenFunc"></param>
        /// <param name="deferredExecution">Should the test be executed immediately or be deferred?</param>
        protected void When(Func<TResult> whenFunc, bool? deferredExecution = null)
        {
            DeferredExecution = deferredExecution ?? DeferredExecution;
            EnsureSubject();
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            
            whenAction = whenFunc.ExecuteInDefaultSynchronizationContext;
            if (!DeferredExecution)
            {
                EnsureTestTriggered(false);
            }
        }

        internal override void TriggerTest(bool expectExceptions)
        {
            TriggerTest(() => result = whenAction(), expectExceptions);
        }

        /// <summary>
        /// Records an asynchronous precondittion
        /// </summary>
        /// <param name="givenFuncASync">The async precondition.</param>
        public void Given(Func<Task> givenFuncASync)
        {
            Given(() => givenFuncASync().GetAwaiter().GetResult());
        }

        /// <summary>
        /// Records a precondition
        /// </summary>
        /// <param name="a"></param>
        public void Given(Action a)
        {
            EnsureContainer();
            a.ExecuteInDefaultSynchronizationContext();
        }
    }

    /// <summary>
    /// BDD style test base class for tests that have a fixed subject. 
    /// The TSubject type defines the type of the subject. 
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject</typeparam>
    public abstract class GivenSubject<TSubject> : TestFor<TSubject> where TSubject : class
    {
        private Action whenAction;

        /// <summary>
        /// The action that triggers the test. Typically used in combination with deferred execution. 
        /// </summary>
        public Action WhenAction
        {
            get => whenAction;
            set
            {
                EnsureSubject(); 
                whenAction = value;
            }
        }

        /// <summary>
        /// Records the action that will trigger the actual test, when later executing the <see cref="WhenAction"/>
        /// </summary>
        public void WhenLater(Action whenAction)
        {
            When(whenAction, deferredExecution: true);
        }

        /// <summary>
        /// Records the asynchronous action that will trigger the actual test, when later executing the <see cref="WhenAction"/>
        /// </summary>
        public void WhenLater(Func<Task> whenActionAsync)
        {
            When(whenActionAsync, deferredExecution: true);
        }

        /// <summary>
        /// Records the asynchronous action that will trigger the actual test
        /// </summary>
        /// <param name="whenActionAsync"></param>
        /// <param name="deferredExecution">Should the test be executed immediately or be deferred?</param>
        public void When(Func<Task> whenActionAsync, bool? deferredExecution = null)
        {
            When(() => whenActionAsync().GetAwaiter().GetResult(), deferredExecution);
        }

        /// <summary>
        /// Records the action that will trigger the actual test
        /// </summary>
        /// <param name="whenAction"></param>
        /// <param name="deferredExecution">Should the test be executed immediately or be deferred?</param>
        public void When(Action whenAction, bool? deferredExecution = null)
        {
            DeferredExecution = deferredExecution ?? DeferredExecution;
            EnsureContainer();
            if (WhenAction != null)
            {
                throw new InvalidOperationException("When already defined");
            }
            this.whenAction = whenAction.ExecuteInDefaultSynchronizationContext;
            if (!DeferredExecution)
            {
                EnsureTestTriggered(false);
            }

        }

        internal override void TriggerTest(bool expectExceptions)
        {
            TriggerTest(whenAction, expectExceptions);
        }

        /// <summary>
        /// Records an asynchronous precondittion
        /// </summary>
        /// <param name="givenFuncASync">The async precondition</param>
        public void Given(Func<Task> givenFuncASync)
        {
            Given(() => givenFuncASync().GetAwaiter().GetResult());
        }

        /// <summary>
        /// Records a precondition
        /// </summary>
        /// <param name="action"></param>
        public void Given(Action action)
        {
            EnsureContainer();
            action.ExecuteInDefaultSynchronizationContext();
        }
    }
}