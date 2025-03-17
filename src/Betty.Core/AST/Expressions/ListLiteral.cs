using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class ListLiteral(List<Expression> elements) : Expression
    {
        public List<Expression> Elements { get; } = elements;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}