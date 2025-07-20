using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class ToListFunction : IntrinsicFunction
    {
        public ToListFunction() : base("tolist") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 1);

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