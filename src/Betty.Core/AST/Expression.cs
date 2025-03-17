using Betty.Core.Interpreter;

namespace Betty.Core.AST
{
    public abstract class Expression
    {
        public abstract Value Accept(IExpressionVisitor visitor);
    }
}