using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static Value RemoveFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 2)
            {
                throw new ArgumentException("remove function requires exactly two arguments: a list and an element to remove.");
            }

            var listResult = call.Arguments[0].Accept(visitor);
            if (listResult.Type != ValueType.List)
            {
                throw new InvalidOperationException("The first argument of remove must be a list.");
            }

            var element = call.Arguments[1].Accept(visitor);
            var list = listResult.AsList();
            list.Remove(element);

            // Return the modified list as a new Value.
            return Value.FromList(new List<Value>(list));
        }
    }
}