namespace Betty.Core.AST
{
    public class CompoundStatement : Statement
    {
        public List<Statement> Statements { get; } = [];

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
    }
}