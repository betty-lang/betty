using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class RemoveAtFunction : IntrinsicFunction
    {
        public RemoveAtFunction() : base("removeat") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 2);

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

            return Value.FromList(new List<Value>(list));
        }
    }
}