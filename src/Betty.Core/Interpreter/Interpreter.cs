using Betty.Core.AST;
using Betty.Core.Interpreter.IntrinsicFunctions;

namespace Betty.Core.Interpreter
{
    public partial class Interpreter(Parser parser) : IStatementVisitor, IExpressionVisitor
    {
        private readonly Parser _parser = parser;
        private readonly Dictionary<string, FunctionDefinition> _functions = [];
        private readonly ScopeManager _scopeManager = new();
        private readonly InterpreterContext _context = new();

        public Value Interpret()
        {
            var tree = _parser.Parse();
            return tree.Accept(this);
        }

        public Value Visit(Program node)
        {
            // Visit each global variable declaration and store it in the global scope
            foreach (var global in node.Globals)
                _scopeManager.DeclareGlobal(global, Value.None()); // Initialize to None

            // Visit each function definition and store it in a dictionary
            foreach (var function in node.Functions)
                function.Accept(this);

            if (_functions.TryGetValue("main", out var mainFunction))
            {
                if (mainFunction.Parameters.Count != 0)
                    throw new Exception("main() function cannot have parameters.");

                // Execute the main function
                return Visit(new FunctionCall([], functionName: "main"));
            }

            throw new Exception("No main function found.");
        }

        public Value Visit(IfExpression node)
        {
            var conditionResult = node.Condition.Accept(this).AsBoolean();

            if (conditionResult)
            {
                return node.ThenExpression.Accept(this);
            }
            else
            {
                foreach (var (Condition, Expression) in node.ElseIfExpressions)
                {
                    if (Condition.Accept(this).AsBoolean())
                    {
                        return Expression.Accept(this);
                    }
                }

                return node.ElseExpression.Accept(this);
            }
        }

        public Value Visit(ListLiteral node)
        {
            var elements = node.Elements.Select(e => e.Accept(this)).ToList();
            return Value.FromList(elements);
        }

        public Value Visit(IndexerExpression node)
        {
            var collection = node.Collection.Accept(this);
            var index = node.Index.Accept(this);

            if (index.Type != ValueType.Number || index.AsNumber() % 1 != 0)
            {
                throw new InvalidOperationException("Index for element access must be an integer.");
            }

            var indexValue = (int)index.AsNumber();

            switch (collection.Type)
            {
                case ValueType.String:
                    var stringValue = collection.AsString();
                    if (indexValue < 0 || indexValue >= stringValue.Length)
                        throw new IndexOutOfRangeException("String index out of range.");
                    return Value.FromChar(stringValue[indexValue]);

                case ValueType.List:
                    var listValue = collection.AsList();
                    if (indexValue < 0 || indexValue >= listValue.Count)
                        throw new IndexOutOfRangeException("List index out of range.");
                    return listValue[indexValue];

                default:
                    throw new InvalidOperationException("Indexing is supported only for lists and strings.");
            }
        }

        public Value Visit(TernaryOperatorExpression node)
        {
            bool conditionResult = node.Condition.Accept(this).AsBoolean();
            return conditionResult ? node.TrueExpression.Accept(this) : node.FalseExpression.Accept(this);
        }

        public void Visit(ReturnStatement node)
        {
            if (node.ReturnValue != null)
            {
                _context.LastReturnValue = node.ReturnValue.Accept(this);
            }
            else
            {
                _context.LastReturnValue = Value.None();
            }
            _context.FlowState = ControlFlowState.Return;
        }

        public void Visit(BreakStatement node)
        {
            if (!_context.IsInLoop)
            {
                throw new Exception("Break statement not inside a loop");
            }
            _context.FlowState = ControlFlowState.Break;
        }

        public void Visit(ContinueStatement node)
        {
            if (!_context.IsInLoop)
            {
                throw new Exception("Continue statement not inside a loop");
            }
            _context.FlowState = ControlFlowState.Continue;
        }

        private bool HandleLoopControlFlow()
        {
            if (_context.FlowState == ControlFlowState.Continue)
            {
                _context.FlowState = ControlFlowState.Normal;
                return false; // Continue to next iteration
            }

            if (_context.FlowState == ControlFlowState.Break)
            {
                _context.FlowState = ControlFlowState.Normal;
                return true; // Break out of loop
            }
            return false;
        }


        public void Visit(DoWhileStatement node)
        {
            _context.EnterLoop();
            _scopeManager.EnterScope();

            do
            {
                node.Body.Accept(this);
                if (HandleLoopControlFlow()) break;
            } while (node.Condition.Accept(this).AsBoolean());

            _scopeManager.ExitScope();
            _context.ExitLoop();
        }

        public void Visit(ForEachStatement node)
        {
            var iterableValue = node.Iterable.Accept(this); // Evaluate the iterable expression

            if (iterableValue.Type != ValueType.List
                && iterableValue.Type != ValueType.String)
            {
                throw new InvalidOperationException("The iterable in a foreach statement must be a list or a string.");
            }

            var list = iterableValue.AsList();

            _context.EnterLoop(); // Enter a new loop context
            _scopeManager.EnterScope();

            foreach (var element in list)
            {
                _scopeManager.SetVariable(node.VariableName, element);
                node.Body.Accept(this); // Execute the body of the loop
                if (HandleLoopControlFlow()) break;
            }

            _scopeManager.ExitScope();
            _context.ExitLoop(); // Exit the loop context
        }

        public void Visit(ForStatement node)
        {
            node.Initializer?.Accept(this);

            _context.EnterLoop();
            _scopeManager.EnterScope();

            while (node.Condition == null || node.Condition.Accept(this).AsBoolean())
            {
                node.Body.Accept(this);
                if (HandleLoopControlFlow()) break;
                node.Increment?.Accept(this);
            }

            _scopeManager.ExitScope();
            _context.ExitLoop();
        }

        public void Visit(WhileStatement node)
        {
            _context.EnterLoop();
            _scopeManager.EnterScope();

            while (node.Condition.Accept(this).AsBoolean())
            {
                node.Body.Accept(this);
                if (HandleLoopControlFlow()) break;
            }

            _scopeManager.ExitScope();
            _context.ExitLoop();
        }

        public void Visit(IfStatement node)
        {
            var conditionResult = node.Condition.Accept(this).AsBoolean();

            if (conditionResult)
            {
                _scopeManager.EnterScope();
                node.ThenStatement.Accept(this);
                _scopeManager.ExitScope();
            }
            else
            {
                bool elseifExecuted = false;
                foreach (var (Condition, Statement) in node.ElseIfStatements)
                {
                    if (Condition.Accept(this).AsBoolean())
                    {
                        _scopeManager.EnterScope();
                        Statement.Accept(this);
                        _scopeManager.ExitScope();
                        elseifExecuted = true;
                        break;
                    }
                }

                if (!elseifExecuted && node.ElseStatement != null)
                {
                    _scopeManager.EnterScope();
                    node.ElseStatement.Accept(this);
                    _scopeManager.ExitScope();
                }
            }
        }

        public Value Visit(BinaryOperatorExpression node) => HandleBinaryExpression(node);

        public Value Visit(BooleanExpression node) => Value.FromBoolean(node.Value);
        public Value Visit(NumberLiteral node) => Value.FromNumber(node.Value);
        public Value Visit(StringLiteral node) => Value.FromString(node.Value);
        public Value Visit(CharLiteral node) => Value.FromChar(node.Value);
        public Value Visit(FunctionExpression node) => Value.FromFunction(node);

        public void Visit(CompoundStatement node)
        {
            _scopeManager.EnterScope();

            foreach (var statement in node.Statements)
            {
                statement.Accept(this);

                if (_context.FlowState != ControlFlowState.Normal)
                {
                    break;
                }
            }

            _scopeManager.ExitScope();
        }

        private static Value ApplyCompoundOperation(Value left, Value right, TokenType operatorType)
        {
            switch (left.Type, right.Type)
            {
                case (ValueType.Number or ValueType.Char, ValueType.Number or ValueType.Char):
                    operatorType = operatorType switch
                    {
                        TokenType.PlusEqual => TokenType.Plus,
                        TokenType.MinusEqual => TokenType.Minus,
                        TokenType.MulEqual => TokenType.Mul,
                        TokenType.DivEqual => TokenType.Div,
                        TokenType.IntDivEqual => TokenType.IntDiv,
                        TokenType.CaretEqual => TokenType.Caret,
                        TokenType.ModEqual => TokenType.Mod,
                        _ => throw new InvalidOperationException("Unsupported compound assignment operator.")
                    };
                    return HandleArithmetic(left, right, operatorType);

                case (ValueType.String, _):
                    if (operatorType != TokenType.PlusEqual)
                        throw new InvalidOperationException("Compound assignment for strings only supports the += operator.");
                    return Value.FromString(left.AsString() + right.ToString());

                case (ValueType.List, _):
                    if (operatorType != TokenType.PlusEqual)
                        throw new InvalidOperationException("Compound assignment for lists only supports the += operator.");

                    var list = left.AsList();

                    if (right.Type == ValueType.List)
                    {
                        list.AddRange(right.AsList());
                    }
                    else
                    {
                        list.Add(right);
                    }
                    return left;

                default:
                    throw new InvalidOperationException("Compound assignment is not supported for the given types.");
            }
        }

        public Value Visit(AssignmentExpression node)
        {
            Value rhsValue = node.Right.Accept(this);

            if (node.Left is Variable variableNode)
            {
                string variableName = variableNode.Name;

                if (node.OperatorType != TokenType.Equal)
                {
                    var lhsValue = _scopeManager.LookupVariable(variableName);
                    rhsValue = ApplyCompoundOperation(lhsValue, rhsValue, node.OperatorType);
                }

                _scopeManager.SetVariable(variableName, rhsValue);
                return rhsValue;
            }
            else if (node.Left is IndexerExpression indexer)
            {
                Value listValue = indexer.Collection.Accept(this);
                Value indexValue = indexer.Index.Accept(this);

                if (listValue.Type != ValueType.List || indexValue.Type != ValueType.Number)
                    throw new InvalidOperationException("Unsupported type or invalid index.");

                List<Value> list = listValue.AsList();
                int index = Convert.ToInt32(indexValue.AsNumber());

                if (index < 0 || index >= list.Count)
                    throw new IndexOutOfRangeException("Index out of range for list assignment.");

                if (node.OperatorType != TokenType.Equal)
                {
                    rhsValue = ApplyCompoundOperation(list[index], rhsValue, node.OperatorType);
                }

                list[index] = rhsValue;
                return rhsValue;
            }
            else
            {
                throw new InvalidOperationException("The left-hand side of an assignment must be a variable or list element.");
            }
        }

        public Value Visit(Variable node) => _scopeManager.LookupVariable(node.Name);

        public void Visit(EmptyStatement node) { }
        
        private Value HandleIncrementDecrement(UnaryOperatorExpression node, Value operandResult)
        {
            var op = node.Operator;
            var fixity = node.Fixity;

            if (node.Operand is Variable variableNode)
            {
                if (operandResult.Type != ValueType.Number && operandResult.Type != ValueType.Char)
                    throw new InvalidOperationException(
                        $"{fixity} {op} operators can only be applied to numbers or characters.");

                var variableName = variableNode.Name;
                var currentValue = operandResult.AsNumber();
                var newValue = op switch
                {
                    TokenType.Increment => currentValue + 1,
                    TokenType.Decrement => currentValue - 1,
                    _ => throw new InvalidOperationException($"Unsupported {fixity} assignment operator {op}")
                };
                _scopeManager.SetVariable(variableName, Value.FromNumber(newValue));

                return node.Fixity == OperatorFixity.Prefix ?
                    Value.FromNumber(newValue) : Value.FromNumber(currentValue);
            }

            if (node.Operand is IndexerExpression indexer)
            {
                var listResult = indexer.Collection.Accept(this);
                var indexResult = indexer.Index.Accept(this);
                if (listResult.Type != ValueType.List || indexResult.Type != ValueType.Number)
                    throw new InvalidOperationException("Invalid element access in list.");

                var list = listResult.AsList();
                var index = (int)indexResult.AsNumber();
                if (index < 0 || index >= list.Count)
                    throw new IndexOutOfRangeException("List index out of range.");

                if (list[index].Type != ValueType.Number
                    && list[index].Type != ValueType.Char)
                    throw new InvalidOperationException(
                        $"{fixity} {op} operators can only be applied to numbers or characters.");

                var currentValue = list[index].AsNumber();
                var newValue = op switch
                {
                    TokenType.Increment => currentValue + 1,
                    TokenType.Decrement => currentValue - 1,
                    _ => throw new InvalidOperationException($"Unsupported {fixity} assignment operator {op}")
                };

                list[index] = Value.FromNumber(newValue);

                return node.Fixity == OperatorFixity.Prefix ?
                    Value.FromNumber(newValue) : Value.FromNumber(currentValue);
            }

            throw new Exception($"The operand of a {fixity} {op} operator must be a variable or a list element.");
        }

        public Value Visit(UnaryOperatorExpression node)
        {
            var operandResult = node.Operand.Accept(this);
            TokenType op = node.Operator;
            OperatorFixity fixity = node.Fixity;

            switch (op, fixity)
            {
                case (TokenType.Plus, OperatorFixity.Prefix):
                    return operandResult;
                case (TokenType.Minus, OperatorFixity.Prefix):
                    return Value.FromNumber(-operandResult.AsNumber());
                case (TokenType.Not, OperatorFixity.Prefix):
                    return Value.FromBoolean(!operandResult.AsBoolean());

                case (TokenType.Increment or TokenType.Decrement, _):
                    return HandleIncrementDecrement(node, operandResult);


                default:
                    throw new InvalidOperationException($"Unsupported {fixity} operator {op}");
            }
        }

        public void Visit(ExpressionStatement node) => node.Expression.Accept(this);
    }
}
