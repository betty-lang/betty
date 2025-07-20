using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class SqrtFunction : IntrinsicFunction
    {
        public SqrtFunction() : base("sqrt") { }

        public override Value Execute(IExpressionVisitor visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 1);

            var argValue = call.Arguments[0].Accept(visitor);
            if (argValue.Type != ValueType.Number)
            {
                throw new ArgumentException($"Argument for {Name} must be a number.");
            }

            double result = Math.Sqrt(argValue.AsNumber());
            return Value.FromNumber(result);
        }
    }
}