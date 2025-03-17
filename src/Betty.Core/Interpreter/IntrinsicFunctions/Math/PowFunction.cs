using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static Value PowFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 2)
            {
                throw new ArgumentException($"{call.FunctionName} function requires two numeric arguments.");
            }

            var baseValue = call.Arguments[0].Accept(visitor);
            var exponentValue = call.Arguments[1].Accept(visitor);
            if (baseValue.Type != ValueType.Number || exponentValue.Type != ValueType.Number)
            {
                throw new ArgumentException($"Arguments for {call.FunctionName} must be numbers.");
            }

            double result = Math.Pow(baseValue.AsNumber(), exponentValue.AsNumber());
            return Value.FromNumber(result);
        }
    }
}