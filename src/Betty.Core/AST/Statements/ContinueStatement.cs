namespace Betty.Core.AST
{
    public class ContinueStatement : Statement
    {
        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}