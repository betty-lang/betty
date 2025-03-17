using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static Value RangeFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 2)
            {
                throw new ArgumentException("range function requires exactly two arguments: a start and an end.");
            }

            var start = call.Arguments[0].Accept(visitor);
            if (start.Type != ValueType.Number)
            {
                throw new InvalidOperationException("The first argument of range must be a number.");
            }

            var end = call.Arguments[1].Accept(visitor);
            if (end.Type != ValueType.Number)
            {
                throw new InvalidOperationException("The second argument of range must be a number.");
            }

            var startValue = start.AsNumber();
            var endValue = end.AsNumber();

            var result = new List<Value>();
            for (var i = startValue; i < endValue; i++)
            {
                result.Add(Value.FromNumber(i));
            }

            return Value.FromList(result);
        }
    }
}