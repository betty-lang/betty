using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class IsDigitFunction : IntrinsicFunction
    {
        public IsDigitFunction() : base("isdigit") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 1);

            var argResult = call.Arguments[0].Accept(visitor);

            if (argResult.Type == ValueType.Char)
            {
                var isDigit = char.IsDigit(argResult.AsChar());
                return Value.FromBoolean(isDigit);
            }

            throw new Exception($"{Name} function is not defined for the given argument type.");
        }
    }
}