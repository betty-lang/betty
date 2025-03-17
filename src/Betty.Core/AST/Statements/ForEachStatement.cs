namespace Betty.Core.AST
{
    public class ForEachStatement(string variableName, Expression iterable, Statement body) : Statement
    {
        public string VariableName { get; } = variableName;
        public Expression Iterable { get; } = iterable;
        public Statement Body { get; } = body;

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}