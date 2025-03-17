using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static Value ToListFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException($"{call.FunctionName} function requires exactly one argument.");
            }

            var argResult = call.Arguments[0].Accept(visitor);

            if (argResult.Type != ValueType.String)
            {
                throw new InvalidOperationException($"Conversion to list not supported for type {argResult.Type}.");
            }

            var str = argResult.AsString();
            var list = new List<Value>();
            foreach (var c in str)
                list.Add(Value.FromChar(c));

            return Value.FromList(list);
        }
    }
}