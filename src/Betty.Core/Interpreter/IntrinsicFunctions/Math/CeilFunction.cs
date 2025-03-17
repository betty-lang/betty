using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static Value CeilFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException($"{call.FunctionName} function requires exactly one numeric argument.");
            }

            var argValue = call.Arguments[0].Accept(visitor);
            if (argValue.Type != ValueType.Number)
            {
                throw new ArgumentException($"Argument for {call.FunctionName} must be a number.");
            }

            double result = Math.Ceiling(argValue.AsNumber());
            return Value.FromNumber(result);
        }
    }
}