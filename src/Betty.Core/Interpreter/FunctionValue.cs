using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public class FunctionValue
    {
        public FunctionExpression Expression { get; }
        public Dictionary<string, Value> CapturedScope { get; }

        public FunctionValue(FunctionExpression expression, Dictionary<string, Value> capturedScope)
        {
            Expression = expression;
            CapturedScope = capturedScope;
        }
    }
}