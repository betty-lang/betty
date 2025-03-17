using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class BinaryOperatorExpression(Expression left, TokenType op, Expression right) : Expression
    {
        public Expression Left { get; } = left;
        public TokenType Operator { get; } = op;
        public Expression Right { get; } = right;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}