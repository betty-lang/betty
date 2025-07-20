using Betty.Core.AST;
using System.Text;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class PrintFunction : IntrinsicFunction
    {
        public PrintFunction() : base("print") { }

        public override Value Execute(IExpressionVisitor<Value> visitor, FunctionCall call)
        {
            var stringBuilder = new StringBuilder();

            foreach (var arg in call.Arguments)
            {
                var argValue = arg.Accept(visitor);
                stringBuilder.Append(argValue.ToString());
            }

            if (call.FunctionName == "println")
                stringBuilder.Append('\n');

            Console.Write(stringBuilder.ToString());

            return Value.None();
        }
    }
}