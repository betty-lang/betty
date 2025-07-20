using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class CosFunction : IntrinsicFunction
    {
        public CosFunction() : base("cos") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 1);

            var argValue = call.Arguments[0].Accept(visitor);
            if (argValue.Type != ValueType.Number)
            {
                throw new ArgumentException($"Argument for {Name} must be a number.");
            }

            double result = Math.Cos(argValue.AsNumber());
            return Value.FromNumber(result);
        }
    }
}