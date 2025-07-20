using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class ToBooleanFunction : IntrinsicFunction
    {
        public ToBooleanFunction() : base("tobool") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 1);

            var argResult = call.Arguments[0].Accept(visitor);

            bool booleanValue;
            switch (argResult.Type)
            {
                case ValueType.Number:
                    booleanValue = argResult.AsNumber() != 0;
                    break;

                case ValueType.Char:
                    booleanValue = true;
                    break;

                case ValueType.String:
                    var str = argResult.AsString();
                    if (bool.TryParse(str, out bool parsedValue))
                    {
                        booleanValue = parsedValue;
                    }
                    else
                    {
                        booleanValue = !string.IsNullOrEmpty(str);
                    }
                    break;

                case ValueType.Boolean:
                    return argResult;

                default:
                    throw new InvalidOperationException($"Conversion to boolean not supported for type {argResult.Type}.");
            }

            return Value.FromBoolean(booleanValue);
        }
    }
}