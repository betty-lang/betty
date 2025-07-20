using Betty.Core.AST;
using Betty.Core.Interpreter.IntrinsicFunctions;

namespace Betty.Core.Interpreter
{
    public partial class Interpreter
    {
        private static readonly Dictionary<string, IntrinsicFunction> _intrinsicFunctions = new()
        {
            // IO
            { "print", new PrintFunction() },
            { "println", new PrintFunction() },
            { "input", new InputFunction() },

            // Conversion
            { "tostr", new ToStringFunction() },
            { "tobool", new ToBooleanFunction() },
            { "tonum", new ToNumberFunction() },
            { "tochar", new ToCharFunction() },
            { "tolist", new ToListFunction() },

            // String
            { "concat", new ConcatFunction() },
            
            // Char
            { "isdigit", new IsDigitFunction() },
            { "isspace", new IsSpaceFunction() },

            // List
            { "append", new AppendFunction() },
            { "range", new RangeFunction() },
            { "remove", new RemoveFunction() },
            { "removeat", new RemoveAtFunction() },
            { "clone", new CloneFunction() },
            { "len", new LengthFunction() },

            // Math
            { "sin", new SinFunction() },
            { "cos", new CosFunction() },
            { "tan", new TanFunction() },
            { "abs", new AbsFunction() },
            { "pow", new PowFunction() },
            { "sqrt", new SqrtFunction() },
            { "floor", new FloorFunction() },
            { "ceil", new CeilFunction() },
        };
    }
}