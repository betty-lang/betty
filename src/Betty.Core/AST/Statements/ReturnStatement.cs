namespace Betty.Core.AST
{
    public class ReturnStatement(Expression? returnValue) : Statement
    {
        public Expression? ReturnValue { get; } = returnValue;

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
    }
}