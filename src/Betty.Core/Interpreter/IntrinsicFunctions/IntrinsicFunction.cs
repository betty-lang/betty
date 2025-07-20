using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public abstract class IntrinsicFunction
    {
        public string Name { get; }

        protected IntrinsicFunction(string name)
        {
            Name = name;
        }

        public abstract Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call);

        protected void ValidateArgumentCount(FunctionCall call, int expectedCount)
        {
            if (call.Arguments.Count != expectedCount)
            {
                throw new ArgumentException($"{Name} function requires exactly {expectedCount} arguments.");
            }
        }

        protected void ValidateArgumentCount(FunctionCall call, int minCount, int maxCount)
        {
            if (call.Arguments.Count < minCount || call.Arguments.Count > maxCount)
            {
                throw new ArgumentException($"{Name} function requires between {minCount} and {maxCount} arguments.");
            }
        }
    }
}
