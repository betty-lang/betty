namespace Betty.Core.AST
{
    public class ForStatement(
        Expression? initializer, 
        Expression? condition,
        Expression? increment, 
        Statement body) : Statement
    {
        public Expression? Initializer { get; } = initializer;
        public Expression? Condition { get; } = condition;
        public Expression? Increment { get; } = increment;
        public Statement Body { get; } = body;

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}