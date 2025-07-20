using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class IfExpression(
        Expression condition, 
        Expression thenExpression,
        List<(Expression Condition, Expression Expression)> elseIfExpressions,
        Expression elseExpression) : Expression
    {
        public Expression Condition { get; } = condition;
        public Expression ThenExpression { get; } = thenExpression;
        public List<(Expression Condition, Expression Expression)> ElseIfExpressions { get; } = elseIfExpressions;
        public Expression ElseExpression { get; } = elseExpression;

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    }
}