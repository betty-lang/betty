namespace Betty.Core.AST
{
    public interface IStatementVisitor
    {
        void Visit(IfStatement node);
        void Visit(ForStatement node);
        void Visit(ForEachStatement node);
        void Visit(WhileStatement node);
        void Visit(DoWhileStatement node);
        void Visit(BreakStatement node);
        void Visit(ContinueStatement node);
        void Visit(ReturnStatement node);
        void Visit(EmptyStatement node);
        void Visit(FunctionDefinition node);
        void Visit(CompoundStatement node);
        void Visit(ExpressionStatement node);
        void Visit(SwitchStatement node);
    }
}