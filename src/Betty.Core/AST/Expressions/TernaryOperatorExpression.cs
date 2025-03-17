using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class TernaryOperatorExpression : Expression
    {
        public Expression Condition { get; }
        public Expression TrueExpression { get; }
        public Expression FalseExpression { get; }

        public TernaryOperatorExpression(Expression condition, Expression trueExpression, Expression falseExpression) 
        {
            Condition = condition;
            TrueExpression = trueExpression;
            FalseExpression = falseExpression;
        }

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}