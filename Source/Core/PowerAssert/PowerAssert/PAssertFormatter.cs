namespace PowerAssertForked
{
    using System;
    using System.Linq.Expressions;
    using Infrastructure;
    using Infrastructure.Nodes;

    internal static class PAssertFormatter
    {
        public static string CreateSimpleFormatFor(LambdaExpression expression)
        {
            Node constantNode = NaturalExpressionParser.Parse(expression.Body);
            return NodeFormatter.SimpleFormat(constantNode);
        }

        public static Exception CreateException(LambdaExpression expression, string message)
        {
            Node constantNode = NaturalExpressionParser.Parse(expression.Body);
            string[] lines = NodeFormatter.Format(constantNode);
            string nl = Environment.NewLine;
            return new Exception(message + ", expression was:" + nl + nl + String.Join(nl, lines));
        }
    }
}
