namespace Betty.Core.AST
{
    public class FunctionDefinition(string? functionName, List<string> parameters, CompoundStatement body) : Statement
    {
        public string? FunctionName { get; } = functionName;
        public List<string> Parameters { get; } = parameters;
        public CompoundStatement Body { get; } = body;

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}