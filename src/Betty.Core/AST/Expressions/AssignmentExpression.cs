using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class AssignmentExpression(Expression left, Expression right, TokenType operatorType) : Expression
    {
        public Expression Left { get; } = left;
        public Expression Right { get; } = right;
        public TokenType OperatorType { get; } = operatorType;

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    }
}