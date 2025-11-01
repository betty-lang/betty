using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class SwitchExpressionCase
    {
        public Expression? CaseExpression { get; }
        public Expression ResultExpression { get; }
        public SwitchExpressionCase(Expression? caseExpression, Expression resultExpression)
        {
            CaseExpression = caseExpression;
            ResultExpression = resultExpression;
        }
    }

    public class SwitchExpression : Expression
    {
        public Expression Expression { get; }
        public List<SwitchExpressionCase> Cases { get; }
        public SwitchExpression(Expression expression, List<SwitchExpressionCase> cases)
        {
            Expression = expression;
            Cases = cases;
        }
        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}