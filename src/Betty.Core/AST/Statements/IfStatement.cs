namespace Betty.Core.AST
{
    public class IfStatement(
        Expression condition, 
        Statement thenStatement,
        List<(Expression Condition, Statement Statement)> elseIfStatements, 
        Statement? elseStatement) : Statement
    {
        public Expression Condition { get; } = condition;
        public Statement ThenStatement { get; } = thenStatement;
        public List<(Expression Condition, Statement Statement)> ElseIfStatements { get; } = elseIfStatements;
        public Statement? ElseStatement { get; } = elseStatement;

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
    }
}