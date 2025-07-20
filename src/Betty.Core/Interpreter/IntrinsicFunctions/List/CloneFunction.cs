using Betty.Core.AST;

namespace Betty.Core.Interpreter.IntrinsicFunctions
{
    public class CloneFunction : IntrinsicFunction
    {
        public CloneFunction() : base("clone") { }

        public override Value Execute(IExpressionVisitor visitor, FunctionCall call)
        {
            ValidateArgumentCount(call, 1);
            var valueToClone = call.Arguments[0].Accept(visitor);
            return valueToClone.Clone();
        }
    }
}
