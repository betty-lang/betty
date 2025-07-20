using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class ToNumberFunction : IntrinsicFunction
    {
        public ToNumberFunction() : base("tonum") { }

        public override Value Execute(IExpressionVisitor visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 1);

            var argResult = call.Arguments[0].Accept(visitor);

            double numberValue;
            switch (argResult.Type)
            {
                case ValueType.Number:
                    return argResult;

                case ValueType.Char:
                    numberValue = argResult.AsNumber();
                    break;

                case ValueType.Boolean:
                    numberValue = argResult.AsBoolean() ? 1 : 0;
                    break;

                case ValueType.String:
                    if (!double.TryParse(argResult.AsString(), out numberValue))
                    {
                        throw new ArgumentException($"Could not convert string '{argResult.AsString()}' to number.");
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Conversion to number not supported for type {argResult.Type}.");
            }

            return Value.FromNumber(numberValue);
        }
    }
}