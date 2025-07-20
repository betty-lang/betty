using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class PowFunction : IntrinsicFunction
    {
        public PowFunction() : base("pow") { }

        public override Value Execute(IExpressionVisitor visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 2);

            var baseValue = call.Arguments[0].Accept(visitor);
            var exponentValue = call.Arguments[1].Accept(visitor);
            if (baseValue.Type != ValueType.Number || exponentValue.Type != ValueType.Number)
            {
                throw new ArgumentException($"Arguments for {Name} must be numbers.");
            }

            double result = Math.Pow(baseValue.AsNumber(), exponentValue.AsNumber());
            return Value.FromNumber(result);
        }
    }
}