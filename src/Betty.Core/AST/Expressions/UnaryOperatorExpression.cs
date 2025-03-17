using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public enum OperatorFixity
    {
        Prefix,
        Postfix
    }

    public class UnaryOperatorExpression(Expression operand, TokenType op, OperatorFixity fixity) : Expression
    {
        public Expression Operand { get; } = operand;
        public TokenType Operator { get; } = op;
        public OperatorFixity Fixity { get; } = fixity;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}