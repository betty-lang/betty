using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static Value CloneFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException("clone function requires exactly one argument: a list to clone.");
            }

            var listResult = call.Arguments[0].Accept(visitor);

            // Allow cloning of lists
            if (listResult.Type != ValueType.List)
            {
                throw new InvalidOperationException("The argument of clone must be a list.");
            }

            // Use the DeepCopy method to create a completely independent copy
            return Value.DeepCopy(listResult);
        }
    }
}