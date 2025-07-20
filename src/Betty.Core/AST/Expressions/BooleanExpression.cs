using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class BooleanExpression(bool value) : Expression
    {
        public bool Value { get; } = value;

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    }
}