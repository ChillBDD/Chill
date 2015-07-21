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
    }
}