namespace Betty.Core.AST
{
    public class ContinueStatement : Statement
    {
        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
    }
}