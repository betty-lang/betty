using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static Value RemoveAtFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 2)
            {
                throw new ArgumentException("removeat function requires exactly two arguments: a list and an index to remove.");
            }

            var listResult = call.Arguments[0].Accept(visitor);
            if (listResult.Type != ValueType.List)
            {
                throw new InvalidOperationException("The first argument of removeat must be a list.");
            }

            var index = call.Arguments[1].Accept(visitor);
            if (index.Type != ValueType.Number)
            {
                throw new InvalidOperationException("The second argument of removeat must be a number.");
            }

            var list = listResult.AsList();
            list.RemoveAt((int)index.AsNumber());

            // Return the modified list as a new Value.
            return Value.FromList(new List<Value>(list));
        }
    }
}