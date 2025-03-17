using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public partial class Interpreter
    {
        private static Value LengthFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            // Ensure that only one argument is provided
            if (call.Arguments.Count != 1)
                throw new Exception($"{call.FunctionName} function requires exactly one argument.");

            // Use the visitor to evaluate the argument
            var argResult = call.Arguments[0].Accept(visitor);

            if (argResult.Type == ValueType.String)
            {
                // Return the length of the string
                var length = argResult.AsString().Length;
                return Value.FromNumber(length);
            }

            if (argResult.Type == ValueType.List)
            {
                // Return the length of the list
                var length = argResult.AsList().Count;
                return Value.FromNumber(length);
            }
            
            throw new Exception($"{call.FunctionName} function is not defined for the given argument type.");
        }
    }
}