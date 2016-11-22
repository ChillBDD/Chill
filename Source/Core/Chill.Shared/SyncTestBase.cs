using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chill.StateBuilders;

namespace Chill
{
    /// <summary>
    /// Base class for all Chill tests except <see cref="AsyncTest" />. This baseclass set's up your automocking container. 
    /// 
    /// It also has a convenient method TriggerTest you can call that will trigger an async test func
    /// and capture any exceptions that might have occurred. 
    /// </summary>
    public abstract partial class SyncTestBase : TestBase
    {
        /// <summary>
        /// Should the test execution start immediately on the When method or should execution be deffered until needed. 
        /// </summary>
        protected bool DefferedExecution { get; set; }
    
        private bool testTriggered;

        /// <summary>
        /// Any exception that might be thrown in the course of executing the When Method. Note, this property is often used
        /// in conjunction with deffered excecution. 
        /// </summary>
        protected Exception CaughtException
        {
            get
            {
                EnsureTestTriggered(expectExceptions: true);
                return caughtException;
            }
            set { caughtException = value; }
        }


        private Exception caughtException;

        /// <summary>
        /// Method that ensures that the test has actually been triggered. 
        /// </summary>
        /// <param name="expectExceptions"></param>
        protected internal void EnsureTestTriggered(bool expectExceptions)
        {
            if (!testTriggered)
            {
                testTriggered = true;
                TriggerTest(expectExceptions);
            }
        }

        /// <summary>
        /// Method that can be overriden to trigger the actual test
        /// </summary>
        /// <param name="expectExceptions"></param>
        internal virtual void TriggerTest(bool expectExceptions)
        {
        }

        internal void TriggerTest(Action testAction, bool expectExceptions)
        {
            if (expectExceptions)
            {
                try
                {
                    testAction();
                }
                catch (AggregateException ex)
                {
                    CaughtException = ex.GetBaseException();
                }
                catch (Exception ex)
                {
                    CaughtException = ex;
                }
                finally
                {
                    if (expectExceptions && CaughtException == null)
                        throw new InvalidOperationException("Expected exception but no exception was thrown");
                }
            }
            else
            {
                testAction();
            }
        }
    }
}