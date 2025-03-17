namespace Betty.Core.AST
{
    public class ReturnStatement(Expression? returnValue) : Statement
    {
        public Expression? ReturnValue { get; } = returnValue;

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}