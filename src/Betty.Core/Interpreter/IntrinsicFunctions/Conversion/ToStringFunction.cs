using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class ToStringFunction : IntrinsicFunction
    {
        public ToStringFunction() : base("tostr") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 1);

            var argResult = call.Arguments[0].Accept(visitor);

            return Value.FromString(argResult.ToString());
        }
    }
}