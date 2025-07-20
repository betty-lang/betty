namespace Betty.Core.AST
{
    public class FunctionDefinition(string? functionName, List<string> parameters, CompoundStatement body) : Statement
    {
        public string? FunctionName { get; } = functionName;
        public List<string> Parameters { get; } = parameters;
        public CompoundStatement Body { get; } = body;

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
    }
}