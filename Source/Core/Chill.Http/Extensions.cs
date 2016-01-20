using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Chill.Http
{
    using System.Linq.Expressions;
    using PowerAssertForked;

    public static class Extensions
    {
        public static string Humanize<TExpression>(this Expression<TExpression> expression)
        {
            return PAssertFormatter.CreateSimpleFormatFor(expression);
        }

        public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T> target)
        {
            if (target == null)
                return Enumerable.Empty<T>();
            return target;
        }
        public static string Humanize(this string lowercaseAndUnderscoredWord)
        {
            return Capitalize(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
        }
        public static string Capitalize(this string word)
        {
            return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
        }
    }
}