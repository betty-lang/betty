using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class AssignmentExpression(Expression left, Expression right, TokenType operatorType) : Expression
    {
        public Expression Left { get; } = left;
        public Expression Right { get; } = right;
        public TokenType OperatorType { get; } = operatorType;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}