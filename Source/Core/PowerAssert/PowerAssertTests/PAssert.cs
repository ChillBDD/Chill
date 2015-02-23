using System;
using System.Linq.Expressions;
using PowerAssert;

namespace PowerAssertTests
{
    public static class PAssert
    {
        public static TException Throws<TException>(Action a) where TException : Exception
        {
            try
            {
                a();
            }
            catch(TException exception)
            {
                return exception;
            }

            throw new Exception("An exception of type " + typeof(TException).Name + " was expected, but no exception occured");
        }

        public static void IsTrue(Expression<Func<bool>> expression)
        {
            Func<bool> func = expression.Compile();
            if (!func())
            {
                throw PAssertFormatter.CreateException(expression, "Assertion failed");
            }
        }
    }
}