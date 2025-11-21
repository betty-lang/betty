namespace Betty.Core.AST
{
    public class ErrorStatement(string message, int line, int column) : Statement
    {
        public string Message { get; } = message;
        public int Line { get; } = line;
        public int Column { get; } = column;

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}
