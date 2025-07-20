using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class TernaryOperatorExpression(Expression condition, Expression trueExpression, Expression falseExpression) : Expression
    {
        public Expression Condition { get; } = condition;
        public Expression TrueExpression { get; } = trueExpression;
        public Expression FalseExpression { get; } = falseExpression;

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    }
}