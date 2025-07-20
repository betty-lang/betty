using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class ListLiteral(List<Expression> elements) : Expression
    {
        public List<Expression> Elements { get; } = elements;

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    }
}