using Betty.Core.Interpreter;
using System.Reflection;

namespace Betty.Core.AST
{
    public class FunctionExpression(List<string> parameters, CompoundStatement body) : Expression
    {
        public List<string> Parameters { get; } = parameters;
        public CompoundStatement Body { get; } = body;

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    }
}