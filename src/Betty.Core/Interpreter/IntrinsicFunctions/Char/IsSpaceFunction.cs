using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static Value IsSpaceFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            // Ensure that only one argument is provided
            if (call.Arguments.Count != 1)
                throw new Exception($"{call.FunctionName} function requires exactly one argument.");

            // Use the visitor to evaluate the argument
            var argResult = call.Arguments[0].Accept(visitor);

            if (argResult.Type == ValueType.Char)
            {
                // Return whether the character is a space
                var isSpace = char.IsWhiteSpace(argResult.AsChar());
                return Value.FromBoolean(isSpace);
            }

            throw new Exception($"{call.FunctionName} function is not defined for the given argument type.");
        }
    }
}