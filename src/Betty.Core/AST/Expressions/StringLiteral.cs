using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class StringLiteral(string value) : Expression
    {
        public string Value { get; } = value;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}