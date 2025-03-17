using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class Variable(string name) : Expression
    {
        public string Name { get; } = name;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}