namespace Betty.Core.AST
{
    public class CompoundStatement : Statement
    {
        public List<Statement> Statements { get; } = [];

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}