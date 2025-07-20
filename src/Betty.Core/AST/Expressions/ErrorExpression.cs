namespace Betty.Core.AST.Expressions;

public class ErrorExpression : Expression
{
    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        // This should not be called.
        // The interpreter should not try to evaluate an error expression.
        throw new NotImplementedException();
    }
}
