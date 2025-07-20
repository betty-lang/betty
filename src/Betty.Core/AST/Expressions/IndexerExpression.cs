using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class IndexerExpression(Expression collection, Expression index) : Expression
    {
        public Expression Collection { get; } = collection;
        public Expression Index { get; } = index;

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    }
}