using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class BooleanExpression(bool value) : Expression
    {
        public bool Value { get; } = value;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}