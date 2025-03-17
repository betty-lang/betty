using Betty.Core.AST;

namespace Betty.Core.Interpreter
{ 
    public static partial class IntrinsicFunctions
    {
        public static Value ToCharFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException($"{call.FunctionName} function requires exactly one argument.");
            }

            var argResult = call.Arguments[0].Accept(visitor);

            char charValue;
            switch (argResult.Type)
            {
                case ValueType.Number:
                    var number = argResult.AsNumber();
                    if (number < char.MinValue || number > char.MaxValue)
                    {
                        throw new ArgumentException($"Number {number} is outside the valid range for characters.");
                    }
                    charValue = (char)number; // Safely cast now that we've checked the range.
                    break;

                case ValueType.Char:
                    return argResult; // No conversion needed.

                case ValueType.String:
                    var stringValue = argResult.AsString();
                    if (stringValue.Length != 1)
                    {
                        throw new ArgumentException($"Could not convert string '{stringValue}' to char.");
                    }
                    charValue = stringValue[0];
                    break;

                case ValueType.Boolean:
                    charValue = argResult.AsBoolean() ? 'T' : 'F';
                    break;

                default:
                    throw new InvalidOperationException($"Conversion to char not supported for type {argResult.Type}.");
            }

            return Value.FromChar(charValue);
        }
    }
}