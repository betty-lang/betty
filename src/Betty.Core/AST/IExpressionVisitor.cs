using Betty.Core.AST.Expressions;

namespace Betty.Core.AST
{
    public interface IExpressionVisitor<T>
    {
        T Visit(NumberLiteral node);
        T Visit(BooleanExpression node);
        T Visit(StringLiteral node);
        T Visit(CharLiteral node);
        T Visit(BinaryOperatorExpression node);
        T Visit(TernaryOperatorExpression node);
        T Visit(UnaryOperatorExpression node);
        T Visit(Variable node);
        T Visit(FunctionCall node);
        T Visit(Program node);
        T Visit(AssignmentExpression node);
        T Visit(IndexerExpression node);
        T Visit(ListLiteral node);
        T Visit(FunctionExpression node);
        T Visit(IfExpression node);
        T Visit(ErrorExpression node);
    }
}