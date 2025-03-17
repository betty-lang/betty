using Betty.Core.AST;
using static Betty.Core.Interpreter.IntrinsicFunctions;

namespace Betty.Core.Interpreter
{
    public partial class Interpreter
    {
        private delegate Value IntrinsicFunctionHandler(FunctionCall call, IExpressionVisitor visitor);

        private static readonly Dictionary<string, IntrinsicFunctionHandler> _intrinsicFunctions = new()
        {
            // IO
            { "print", PrintFunction },
            { "println", PrintFunction },
            { "input", InputFunction },

            // Conversion
            { "tostr", ToStringFunction },
            { "tobool", ToBooleanFunction },
            { "tonum", ToNumberFunction },
            { "tochar", ToCharFunction },
            { "tolist", ToListFunction },

            // String
            { "concat", ConcatFunction },
            { "len", LengthFunction },

            // Char
            { "isdigit", IsDigitFunction },
            { "isspace", IsSpaceFunction },

            // List
            { "append", AppendFunction },
            { "range", RangeFunction },
            { "remove", RemoveFunction },
            { "removeat", RemoveAtFunction },
            { "clone", CloneFunction },

            // Math
            { "sin", SinFunction },
            { "cos", CosFunction },
            { "tan", TanFunction },
            { "abs", AbsFunction },
            { "pow", PowFunction },
            { "sqrt", SqrtFunction },
            { "floor", FloorFunction },
            { "ceil", CeilFunction },
        };
    }
}