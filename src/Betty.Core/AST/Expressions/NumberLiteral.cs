using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class NumberLiteral(double value) : Expression
    {
        public double Value { get; } = value;

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    }
}