using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class TernaryOperatorExpression(Expression condition, Expression trueExpression, Expression falseExpression) : Expression
    {
        public Expression Condition { get; } = condition;
        public Expression TrueExpression { get; } = trueExpression;
        public Expression FalseExpression { get; } = falseExpression;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}