using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class LengthFunction : IntrinsicFunction
    {
        public LengthFunction() : base("len") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 1);

            var argResult = call.Arguments[0].Accept(visitor);

            if (argResult.Type == ValueType.String)
            {
                var length = argResult.AsString().Length;
                return Value.FromNumber(length);
            }

            if (argResult.Type == ValueType.List)
            {
                var length = argResult.AsList().Count;
                return Value.FromNumber(length);
            }

            throw new Exception($"{Name} function is not defined for the given argument type.");
        }
    }
}
