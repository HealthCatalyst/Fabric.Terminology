namespace Fabric.Terminology.SqlServer
{
    using System;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// A general-purpose Expression Visitor that allows a delegate to replace any given expression with another expression.
    /// </summary>
    internal class ExpressionReplacer : ExpressionVisitor
    {
        private readonly Func<Expression, Expression> replacer;

        public ExpressionReplacer(Func<Expression, Expression> replacer)
        {
            this.replacer = replacer;
        }

        public override Expression Visit([NotNull]Expression node)
        {
            return base.Visit(this.replacer(node));
        }
    }
}