namespace Betty.Core.AST
{
    public class DoWhileStatement(Expression condition, Statement body) : Statement
    {
        public Expression Condition { get; } = condition;
        public Statement Body { get; } = body;

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}