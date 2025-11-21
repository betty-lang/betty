using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class ErrorExpression(string message, int line, int column) : Expression
    {
        public string Message { get; } = message;
        public int Line { get; } = line;
        public int Column { get; } = column;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}
