namespace Betty.Core.AST
{
    public class BreakStatement : Statement
    {
        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}