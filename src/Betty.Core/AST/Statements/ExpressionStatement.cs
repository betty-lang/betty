namespace Betty.Core.AST
{
    public class ExpressionStatement(Expression expression) : Statement
    {
        public Expression Expression { get; } = expression;

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}