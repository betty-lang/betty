namespace Betty.Core.AST
{
    public class ExpressionStatement(Expression expression) : Statement
    {
        public Expression Expression { get; } = expression;

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
    }
}