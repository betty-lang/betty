namespace Betty.Core.AST
{
    public class EmptyStatement : Statement
    {
        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
    }
}