using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class IsSpaceFunction : IntrinsicFunction
    {
        public IsSpaceFunction() : base("isspace") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 1);

            var argResult = call.Arguments[0].Accept(visitor);

            if (argResult.Type == ValueType.Char)
            {
                var isSpace = char.IsWhiteSpace(argResult.AsChar());
                return Value.FromBoolean(isSpace);
            }

            throw new Exception($"{Name} function is not defined for the given argument type.");
        }
    }
}