namespace Betty.Core.AST
{
    public interface IStatementVisitor<T>
    {
        T Visit(IfStatement node);
        T Visit(ForStatement node);
        T Visit(ForEachStatement node);
        T Visit(WhileStatement node);
        T Visit(DoWhileStatement node);
        T Visit(BreakStatement node);
        T Visit(ContinueStatement node);
        T Visit(ReturnStatement node);
        T Visit(EmptyStatement node);
        T Visit(FunctionDefinition node);
        T Visit(CompoundStatement node);
        T Visit(ExpressionStatement node);
    }
}