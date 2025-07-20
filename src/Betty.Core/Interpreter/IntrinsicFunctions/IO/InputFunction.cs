using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class InputFunction : IntrinsicFunction
    {
        public InputFunction() : base("input") { }

        public override Value Execute(IExpressionVisitor visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 0, 1);

            if (call.Arguments.Count == 1)
            {
                var promptValue = call.Arguments[0].Accept(visitor);
                Console.Write(promptValue.AsString());
            }

            string userInput = Console.ReadLine() ?? string.Empty;

            return Value.FromString(userInput);
        }
    }
}