namespace Betty.Core.AST
{
    public class IfStatement : Statement
    {
        public Expression Condition { get; }
        public Statement ThenStatement { get; }
        public List<(Expression Condition, Statement Statement)> ElseIfStatements { get; }
        public Statement? ElseStatement { get; }

        public IfStatement(Expression condition, Statement thenStatement,
            List<(Expression Condition, Statement Statement)> elseIfStatements, Statement? elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseIfStatements = elseIfStatements ?? [];
            ElseStatement = elseStatement;
        }

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}