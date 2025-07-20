using Betty.Core.AST;
using System.Text;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class ConcatFunction : IntrinsicFunction
    {
        public ConcatFunction() : base("concat") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            var stringBuilder = new StringBuilder();

            foreach (var arg in call.Arguments)
            {
                var argValue = arg.Accept(visitor);
                stringBuilder.Append(argValue.ToString());
            }

            return Value.FromString(stringBuilder.ToString());
        }
    }
}