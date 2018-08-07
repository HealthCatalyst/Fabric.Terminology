namespace Fabric.Terminology.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal static class ExpressionHelpers
    {
        /// <summary>
        /// Replaces any occurrences of the <paramref name="toReplace"/> parameter in the given expression
        /// with the <paramref name="replacement"/> parameter.
        /// </summary>
        public static T ReplaceParameter<T>(T expression, ParameterExpression toReplace, ParameterExpression replacement)
            where T : Expression
        {
            var replacer = new ExpressionReplacer(e => e == toReplace ? replacement : e);
            return (T)replacer.Visit(expression);
        }

        /// <summary>
        /// Joins the bodies of the given lambda expressions together using the given <paramref name="joiner"/> to produce a single lambda expression.
        /// </summary>
        /// <example><code>var searchExpression = ExpressionHelpers.Join(Expressions.Or, searchCriteria);</code></example>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="expressions"/> is empty.
        /// </exception>
        public static Expression<Func<T, TReturn>> Join<T, TReturn>(Func<Expression, Expression, BinaryExpression> joiner, IReadOnlyCollection<Expression<Func<T, TReturn>>> expressions)
        {
            if (!expressions.Any())
            {
                throw new ArgumentException("No expressions were provided");
            }
            var firstExpression = expressions.First();
            var otherExpressions = expressions.Skip(1);
            var firstParameter = firstExpression.Parameters.Single();
            var otherExpressionsWithParameterReplaced = otherExpressions.Select(e => ReplaceParameter(e.Body, e.Parameters.Single(), firstParameter));
            var bodies = new[] { firstExpression.Body }.Concat(otherExpressionsWithParameterReplaced);
            var joinedBodies = bodies.Aggregate(joiner);
            return Expression.Lambda<Func<T, TReturn>>(joinedBodies, firstParameter);
        }
    }
}