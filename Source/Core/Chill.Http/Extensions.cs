namespace Chill.Http
{
    using System.Linq.Expressions;

    public static class Extensions
    {
        public static string Humanize<TExpression>(this Expression<TExpression> expression)
        {
            return expression.ToString();
        }
    }
}