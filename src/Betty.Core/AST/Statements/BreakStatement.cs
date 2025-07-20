namespace Betty.Core.AST
{
    public class BreakStatement : Statement
    {
        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
    }
}