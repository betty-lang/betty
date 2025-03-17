using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static Value ConcatFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            // Concatenate all arguments into a single string
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var arg in call.Arguments)
            {
                var argValue = arg.Accept(visitor);
                stringBuilder.Append(argValue.ToString());
            }

            return Value.FromString(stringBuilder.ToString());
        }
    }
}