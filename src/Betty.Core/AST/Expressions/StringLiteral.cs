using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class StringLiteral(string value) : Expression
    {
        public string Value { get; } = value;

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    }
}