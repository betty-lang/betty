namespace Betty.Core.AST
{
    public class ForStatement : Statement
    {
        public Expression? Initializer { get; }
        public Expression? Condition { get; }
        public Expression? Increment { get; }
        public Statement Body { get; }

        public ForStatement(Expression? initializer, Expression? condition, 
            Expression? increment, Statement body)
        {
            Initializer = initializer;
            Condition = condition;
            Increment = increment;
            Body = body;
        }

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}