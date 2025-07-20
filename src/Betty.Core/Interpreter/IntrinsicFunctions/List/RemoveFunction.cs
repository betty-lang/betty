using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class RemoveFunction : IntrinsicFunction
    {
        public RemoveFunction() : base("remove") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 2);

            var listResult = call.Arguments[0].Accept(visitor);
            if (listResult.Type != ValueType.List)
            {
                throw new InvalidOperationException("The first argument of remove must be a list.");
            }

            var element = call.Arguments[1].Accept(visitor);
            var list = listResult.AsList();
            list.Remove(element);

            return Value.FromList(new List<Value>(list));
        }
    }
}