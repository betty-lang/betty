namespace Betty.Core.AST
{
    public class SwitchCase
    {
        public Expression? CaseExpression { get; }
        public List<Statement> Statements { get; }
        public SwitchCase(Expression? caseExpression, List<Statement> statements)
        {
            CaseExpression = caseExpression;
            Statements = statements;
        }
    }

    public class SwitchStatement : Statement
    {
        public Expression Expression { get; }
        public List<SwitchCase> Cases { get; }
        public SwitchStatement(Expression expression, List<SwitchCase> cases)
        {
            Expression = expression;
            Cases = cases;
        }
        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}