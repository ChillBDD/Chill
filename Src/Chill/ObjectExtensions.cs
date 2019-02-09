using System;
using System.Threading.Tasks;

namespace Chill
{
    /// <summary>
    /// Extensions that will help to make the Chill syntax a bit more fluent. 
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// The With extension method allows you to further configure an already existing object (or an object created by a mother)
        /// For example:  mother.CreateCustomer().With(x=> x.Name = Erwin).And(x => x.Address = "address") 
        /// </summary>
        /// <typeparam name="TSubject"></typeparam>
        /// <typeparam name="TReturnValue"></typeparam>
        /// <param name="subject"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TSubject With<TSubject, TReturnValue>(this TSubject subject, Func<TSubject, TReturnValue> func)
        {
            func(subject);
            return subject;
        }

        /// <summary>
        /// Exactly the same method as With, but then allows you to chain multiple withs more fluently. 
        /// .With().And().And(); 
        /// </summary>
        /// <typeparam name="TSubject"></typeparam>
        /// <typeparam name="TReturnValue"></typeparam>
        /// <param name="subject"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TSubject And<TSubject, TReturnValue>(this TSubject subject, Func<TSubject, TReturnValue> func)
        {
            func(subject);
            return subject;
        }
    }
}