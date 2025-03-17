using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class CharLiteral(char value) : Expression
    {
        public char Value { get; } = value;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}