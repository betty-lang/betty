using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class AppendFunction : IntrinsicFunction
    {
        public AppendFunction() : base("append") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 2);

            var listResult = call.Arguments[0].Accept(visitor);
            if (listResult.Type != ValueType.List)
            {
                throw new InvalidOperationException("The first argument of append must be a list.");
            }

            var element = call.Arguments[1].Accept(visitor);
            var list = listResult.AsList();
            list.Add(element);

            return Value.FromList(new List<Value>(list));
        }
    }
}