using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class NumberLiteral(double value) : Expression
    {
        public double Value { get; } = value;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}