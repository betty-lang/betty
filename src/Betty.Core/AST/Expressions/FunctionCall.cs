using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public class FunctionCall(List<Expression> arguments, 
        Expression? expression = null,
        string? functionName = null) : Expression
    {
        public List<Expression> Arguments { get; } = arguments;
        public Expression? Expression = expression;
        public string? FunctionName = functionName;

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    }
}