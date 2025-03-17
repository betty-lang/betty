using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class IfExpression : Expression
    {
        public Expression Condition { get; }
        public Expression ThenExpression { get; }
        public List<(Expression Condition, Expression Expression)> ElseIfExpressions { get; }
        public Expression ElseExpression { get; }

        public IfExpression(Expression condition, Expression thenExpression,
            List<(Expression Condition, Expression Expression)> elseIfExpressions,
            Expression elseExpression)
        {
            Condition = condition;
            ThenExpression = thenExpression;
            ElseIfExpressions = elseIfExpressions ?? [];
            ElseExpression = elseExpression;
        }

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}