using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class RangeFunction : IntrinsicFunction
    {
        public RangeFunction() : base("range") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 2);

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