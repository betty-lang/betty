using Betty.Core.AST;

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
                return Visit(new FunctionCall([], functionName : "main"));
            }
            
            throw new Exception("No main function found.");
        }

        public Value Visit(SwitchExpression node)
        {
            var switchValue = node.Expression.Accept(this);
            foreach (var switchCase in node.Cases)
            {
                // Check if this case matches
                if (switchCase.CaseExpression == null) // default case
                {
                    return switchCase.ResultExpression.Accept(this);
                }
                else
                {
                    var caseValue = switchCase.CaseExpression.Accept(this);
                    if (switchValue == caseValue)
                    {
                        return switchCase.ResultExpression.Accept(this);
                    }
                }
            }
            // If no cases matched, return None
            return Value.None();
        }

        public void Visit(SwitchStatement node)
        {
            var switchValue = node.Expression.Accept(this);
            bool shouldExecute = false;

            _context.EnterSwitch(); // Track that we're in a switch

            foreach (var switchCase in node.Cases)
            {
                // Check if this case matches (or if we're falling through)
                if (switchCase.CaseExpression == null) // default case
                {
                    shouldExecute = true;
                }
                else if (!shouldExecute) // only check if we haven't matched yet
                {
                    var caseValue = switchCase.CaseExpression.Accept(this);
                    shouldExecute = switchValue == caseValue;
                }

                // Execute case body if we should
                if (shouldExecute)
                {
                    foreach (var statement in switchCase.Statements)
                    {
                        statement.Accept(this);

                        // Check for control flow changes
                        if (_context.FlowState == ControlFlowState.Break)
                        {
                            _context.FlowState = ControlFlowState.Normal; // Consume the break
                            return; // Exit the switch
                        }
                        else if (_context.FlowState != ControlFlowState.Normal)
                        {
                            return; // Return or continue, exit switch
                        }
                    }
                    // If we reach here, fall through to next case
                }
            }

            _context.ExitSwitch(); // Exit switch context
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
            if (!_context.CanBreak)
            {
                throw new Exception("Break statement must be inside a loop or switch statement");
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

        public void Visit(DoWhileStatement node)
        {
            _context.EnterLoop();

            _scopeManager.EnterScope();

            node.Body.Accept(this);

            while (node.Condition.Accept(this).AsBoolean())
            {
                node.Body.Accept(this);

                if (_context.FlowState == ControlFlowState.Continue)
                {
                    _context.FlowState = ControlFlowState.Normal;
                }
                else if (_context.FlowState == ControlFlowState.Break)
                {
                    _context.FlowState = ControlFlowState.Normal;
                    break;
                }
            }

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
                // Set the loop variable to the current element
                _scopeManager.SetVariable(node.VariableName, element);

                node.Body.Accept(this); // Execute the body of the loop

                if (_context.FlowState == ControlFlowState.Continue)
                {
                    _context.FlowState = ControlFlowState.Normal;
                }
                else if (_context.FlowState == ControlFlowState.Break)
                {
                    _context.FlowState = ControlFlowState.Normal;
                    break;
                }
            }

            _scopeManager.ExitScope();

            _context.ExitLoop(); // Exit the loop context
        }

        public void Visit(ForStatement node)
        {
            // Execute the initializer once before the loop starts.
            node.Initializer?.Accept(this);

            _context.EnterLoop(); // Enter a new loop context.

            _scopeManager.EnterScope();

            while (node.Condition == null || node.Condition.Accept(this).AsBoolean())
            {
                // Execute the body of the loop.
                node.Body.Accept(this);

                // If a continue statement was encountered, reset the flow state and skip directly to the increment.
                if (_context.FlowState == ControlFlowState.Continue)
                {
                    _context.FlowState = ControlFlowState.Normal; // Reset the flow state.
                }
                else if (_context.FlowState == ControlFlowState.Break)
                {
                    // If a break statement was encountered, exit the loop.
                    _context.FlowState = ControlFlowState.Normal; // Reset the flow state.
                    break;
                }

                // Execute the increment expression.
                node.Increment?.Accept(this);
            }

            _scopeManager.ExitScope();

            _context.ExitLoop(); // Exit the loop context.
        }

        public void Visit(WhileStatement node)
        {
            _context.EnterLoop();

            _scopeManager.EnterScope();

            while (node.Condition.Accept(this).AsBoolean())
            {
                // Execute the body of the loop.
                node.Body.Accept(this);

                // Handle continue
                if (_context.FlowState == ControlFlowState.Continue)
                {
                    _context.FlowState = ControlFlowState.Normal; // Reset the flow state.
                }
                else if (_context.FlowState == ControlFlowState.Break)
                {
                    // If a break statement was encountered, exit the loop.
                    _context.FlowState = ControlFlowState.Normal; // Reset the flow state.
                    break;
                }
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
                        break; // Exit after the first true elseif to prevent executing multiple blocks
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

        public Value Visit(BinaryOperatorExpression node)
        {
            var leftResult = node.Left.Accept(this);
            var rightResult = node.Right.Accept(this);

            switch (node.Operator)
            {
                case TokenType.And:
                case TokenType.Or:
                    if (leftResult.Type != ValueType.Boolean || rightResult.Type != ValueType.Boolean)
                    {
                        throw new InvalidOperationException("Logical operations require both operands to be boolean.");
                    }
                    bool leftBoolean = leftResult.AsBoolean();
                    bool rightBoolean = rightResult.AsBoolean();
                    return Value.FromBoolean(
                        node.Operator == TokenType.And ? leftBoolean && rightBoolean : leftBoolean || rightBoolean);

                case TokenType.Plus:
                    // If either operand is a string, concatenate as strings
                    if (leftResult.Type == ValueType.String || rightResult.Type == ValueType.String)
                    {
                        return Value.FromString(leftResult.ToString() + rightResult.ToString());
                    }
                    // If the left operand is a list, add the right operand to the list
                    if (leftResult.Type == ValueType.List)
                    {
                        var list = leftResult.AsList();
                        // If right is also a list, extend with its items
                        if (rightResult.Type == ValueType.List)
                        {
                            list.AddRange(rightResult.AsList());
                        }
                        else
                        {
                            list.Add(rightResult);
                        }
                        return leftResult;
                    }
                    // If the right operand is a list, add the left operand to the list
                    if (rightResult.Type == ValueType.List)
                    {
                        var list = rightResult.AsList();
                        // If left is also a list, prepend its items
                        if (leftResult.Type == ValueType.List)
                        {
                            list.InsertRange(0, leftResult.AsList());
                        }
                        else
                        {
                            list.Insert(0, leftResult);
                        }
                        return rightResult;
                    }
                    // If none are strings or lists, perform numerical addition
                    return Value.FromNumber(leftResult.AsNumber() + rightResult.AsNumber());
                case TokenType.Minus:
                case TokenType.Mul:
                case TokenType.Div:
                case TokenType.IntDiv:
                case TokenType.Caret:
                case TokenType.Mod:
                    return (leftResult.Type, rightResult.Type) switch 
                    {
                        (ValueType.Number or ValueType.Char, ValueType.Number or ValueType.Char) =>
                            PerformArithmeticOperation(leftResult, rightResult, node.Operator),
                        _ => throw new InvalidOperationException("Arithmetic operations require both operands to be numbers.")
                    };

                case TokenType.EqualEqual:
                case TokenType.NotEqual:
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEqual:
                    return PerformComparison(leftResult, rightResult, node.Operator);

                default:
                    throw new Exception($"Unsupported binary operator: {node.Operator} for operand types {leftResult.Type} and {rightResult.Type}");
            }
        }

        private static Value PerformArithmeticOperation(Value leftResult, Value rightResult, TokenType operatorType)
        {
            double leftNumber = leftResult.AsNumber();
            double rightNumber = rightResult.AsNumber();

            return operatorType switch
            {
                TokenType.Plus => Value.FromNumber(leftNumber + rightNumber),
                TokenType.Minus => Value.FromNumber(leftNumber - rightNumber),
                TokenType.Mul => Value.FromNumber(leftNumber * rightNumber),
                TokenType.Div => Value.FromNumber(leftNumber / rightNumber),
                TokenType.IntDiv => Value.FromNumber(Math.Floor(leftNumber / rightNumber)),
                TokenType.Caret => Value.FromNumber(Math.Pow(leftNumber, rightNumber)),
                TokenType.Mod => Value.FromNumber(leftNumber % rightNumber),
                _ => throw new Exception($"Unsupported arithmetic operator: {operatorType}")
            };
        }

        private static Value PerformComparison(Value leftResult, Value rightResult, TokenType operatorType)
        {
            switch (leftResult.Type, rightResult.Type)
            {
                case (ValueType.Number or ValueType.Char, ValueType.Number or ValueType.Char):
                    double leftNumber = leftResult.AsNumber();
                    double rightNumber = rightResult.AsNumber();
                    return Value.FromBoolean(operatorType switch
                    {
                        TokenType.EqualEqual => leftNumber == rightNumber,
                        TokenType.NotEqual => leftNumber != rightNumber,
                        TokenType.LessThan => leftNumber < rightNumber,
                        TokenType.LessThanOrEqual => leftNumber <= rightNumber,
                        TokenType.GreaterThan => leftNumber > rightNumber,
                        TokenType.GreaterThanOrEqual => leftNumber >= rightNumber,
                        _ => throw new Exception($"Unsupported operator for number comparison: {operatorType}")
                    });

                case (ValueType.String, ValueType.String):
                    return Value.FromBoolean(operatorType switch
                    {
                        TokenType.EqualEqual => leftResult == rightResult,
                        TokenType.NotEqual => leftResult != rightResult,
                        TokenType.LessThan => leftResult.AsString().CompareTo(rightResult.AsString()) < 0,
                        TokenType.LessThanOrEqual => leftResult.AsString().CompareTo(rightResult.AsString()) <= 0,
                        TokenType.GreaterThan => leftResult.AsString().CompareTo(rightResult.AsString()) > 0,
                        TokenType.GreaterThanOrEqual => leftResult.AsString().CompareTo(rightResult.AsString()) >= 0,
                        _ => throw new Exception($"Unsupported operator for string comparison: {operatorType}")
                    });

                case (ValueType.Boolean, ValueType.Boolean):
                    return Value.FromBoolean(operatorType switch
                    {
                        TokenType.EqualEqual => leftResult == rightResult,
                        TokenType.NotEqual => leftResult != rightResult,
                        _ => throw new Exception($"Unsupported operator for boolean comparison: {operatorType}")
                    });

                case (ValueType.List, ValueType.List) when operatorType == TokenType.EqualEqual || operatorType == TokenType.NotEqual:
                    var leftList = leftResult.AsList();
                    var rightList = rightResult.AsList();

                    // Short-circuit evaluation for lists of different lengths
                    if (leftList.Count != rightList.Count)
                        return Value.FromBoolean(operatorType == TokenType.NotEqual);

                    // Perform element-wise comparison
                    for (int i = 0; i < leftList.Count; i++)
                    {
                        // Use the equality operator for element comparison
                        var elementComparisonResult = PerformComparison(leftList[i], rightList[i], TokenType.EqualEqual);
                        if (!elementComparisonResult.AsBoolean())
                        {
                            // If any element comparison returns false for equality, the lists are not equal
                            return Value.FromBoolean(operatorType == TokenType.NotEqual);
                        }
                    }

                    // If we reach here, all elements are equal
                    return Value.FromBoolean(operatorType == TokenType.EqualEqual);

                case (ValueType.Function, ValueType.Function) when operatorType == TokenType.EqualEqual || operatorType == TokenType.NotEqual:
                    var leftFunction = leftResult.AsFunction();
                    var rightFunction = rightResult.AsFunction();
                    return Value.FromBoolean(leftFunction == rightFunction);

                default:
                    throw new Exception("Type mismatch or unsupported types for comparison.");
            }
        }

        public Value Visit(BooleanExpression node) => Value.FromBoolean(node.Value);
        public Value Visit(NumberLiteral node) => Value.FromNumber(node.Value);
        public Value Visit(StringLiteral node) => Value.FromString(node.Value);
        public Value Visit(CharLiteral node) => Value.FromChar(node.Value);
        public Value Visit(FunctionExpression node)
        {
            // Capture current scope
            var capturedScope = _scopeManager.GetAllVariables();
            return Value.FromFunction(node, capturedScope);
        }

        public void Visit(CompoundStatement node)
        {
            _scopeManager.EnterScope();

            foreach (var statement in node.Statements)
            {
                statement.Accept(this);

                // Handle control flow changes
                if (_context.FlowState != ControlFlowState.Normal)
                {
                    break; // Exit the compound statement early
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
                    return PerformArithmeticOperation(left, right, operatorType);

                case (ValueType.String, _):
                    if (operatorType != TokenType.PlusEqual)
                        throw new InvalidOperationException("Compound assignment for strings only supports the += operator.");
                    return Value.FromString(left.AsString() + right.ToString());

                case (ValueType.List, _):
                    if (operatorType != TokenType.PlusEqual)
                        throw new InvalidOperationException("Compound assignment for lists only supports the += operator.");

                    // Mutate the original list in-place
                    var list = left.AsList();

                    // If the right value is a list, extend the list
                    if (right.Type == ValueType.List)
                    {
                        list.AddRange(right.AsList());
                    }
                    // Otherwise, add the single item
                    else
                    {
                        list.Add(right);
                    }

                    // Return the same Value, which now contains the mutated list
                    return left;

                default:
                    throw new InvalidOperationException("Compound assignment is not supported for the given types.");
            }
        }

        public Value Visit(AssignmentExpression node)
        {
            // Evaluate RHS
            Value rhsValue = node.Right.Accept(this);

            // Simple variable assignment
            if (node.Left is Variable variableNode)
            {
                string variableName = variableNode.Name;

                // If compound assignment, get current value
                if (node.OperatorType != TokenType.Equal)
                {
                    var lhsValue = _scopeManager.LookupVariable(variableName);
                    rhsValue = ApplyCompoundOperation(lhsValue, rhsValue, node.OperatorType);
                }

                // Perform the assignment
                _scopeManager.SetVariable(variableName, rhsValue);
                return rhsValue;
            }
            // Assignment to a list element
            else if (node.Left is IndexerExpression indexer)
            {
                Value listValue = indexer.Collection.Accept(this);
                Value indexValue = indexer.Index.Accept(this);

                if (listValue.Type != ValueType.List || indexValue.Type != ValueType.Number)
                    throw new InvalidOperationException("Unsupported type or invalid index.");

                List<Value> list = listValue.AsList();
                int index = Convert.ToInt32(indexValue.AsNumber());

                // Ensure index is within bounds
                if (index < 0 || index >= list.Count)
                    throw new IndexOutOfRangeException("Index out of range for list assignment.");

                // If compound assignment, apply the operator to the current element
                if (node.OperatorType != TokenType.Equal)
                {
                    rhsValue = ApplyCompoundOperation(list[index], rhsValue, node.OperatorType);
                }

                // Update the list element
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
        public Value Visit(FunctionCall node)
        {
            FunctionDefinition? function = null;
            Dictionary<string, Value>? capturedScope = null;

            // Resolve function reference
            if (node.FunctionName is not null)
            {
                if (_intrinsicFunctions.TryGetValue(node.FunctionName, out var intrinsicFunction))
                    return intrinsicFunction.Invoke(node, this);

                if (_functions.TryGetValue(node.FunctionName, out var globalFunction))
                {
                    function = globalFunction;
                    // Global functions don't have captured scopes (or capture empty scope)
                    capturedScope = new Dictionary<string, Value>();
                }
                else
                {
                    var funcValue = _scopeManager.LookupVariable(node.FunctionName).AsFunction();
                    function = new FunctionDefinition(null, funcValue.Expression.Parameters, funcValue.Expression.Body);
                    capturedScope = funcValue.CapturedScope;
                }
            }
            else if (node.Expression is FunctionExpression funcExpr)
            {
                // Handle inline function expressions like (func() { ... })()
                // These are evaluated on the spot, so capture current scope
                var funcValue = funcExpr.Accept(this).AsFunction();
                function = new FunctionDefinition(null, funcValue.Expression.Parameters, funcValue.Expression.Body);
                capturedScope = funcValue.CapturedScope;
            }
            else if (node.Expression is IndexerExpression or FunctionCall)
            {
                var funcValue = node.Expression.Accept(this).AsFunction();
                function = new FunctionDefinition(null, funcValue.Expression.Parameters, funcValue.Expression.Body);
                capturedScope = funcValue.CapturedScope;
            }

            if (function is null)
                throw new Exception($"Function not found: {node.FunctionName ?? "anonymous function"}");

            // Enter function scope
            _scopeManager.EnterScope();

            // Restore captured scope FIRST (use isFunctionParam = false so they can be reassigned)
            if (capturedScope != null)
            {
                foreach (var kvp in capturedScope)
                {
                    _scopeManager.SetVariable(kvp.Key, kvp.Value, false);
                }
            }

            // Save execution context
            int previousLoopDepth = _context.LoopDepth;
            _context.LoopDepth = 0;
            var previousFlowState = _context.FlowState;
            _context.FlowState = ControlFlowState.Normal;

            // Bind function arguments (these CAN shadow captured variables)
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var argValue = node.Arguments[i].Accept(this);
                _scopeManager.SetVariable(function.Parameters[i], argValue, true);
            }

            // Execute function body
            function.Body.Accept(this);

            // Retrieve return value
            Value returnValue = (_context.FlowState == ControlFlowState.Return)
                ? _context.LastReturnValue
                : Value.None();

            // Restore previous execution context and exit function scope
            _context.LoopDepth = previousLoopDepth;
            _context.FlowState = previousFlowState;
            _scopeManager.ExitScope();

            return returnValue;
        }

        public void Visit(FunctionDefinition node)
        {
            if (_intrinsicFunctions.ContainsKey(node.FunctionName!))
                throw new Exception($"Function name '{node.FunctionName}' is reserved for built-in functions.");

            _functions[node.FunctionName!] = node;
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

                    // Check if the operand is an element access in a list
                    if (node.Operand is IndexerExpression indexer)
                    {
                        // Ensure the list and index are valid
                        var listResult = indexer.Collection.Accept(this);
                        var indexResult = indexer.Index.Accept(this);
                        if (listResult.Type != ValueType.List || indexResult.Type != ValueType.Number)
                            throw new InvalidOperationException("Invalid element access in list.");

                        var list = listResult.AsList();
                        var index = (int)indexResult.AsNumber();
                        if (index < 0 || index >= list.Count)
                            throw new IndexOutOfRangeException("List index out of range.");

                        // Ensure the target element is a number or a character
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

                        // Update the list element
                        list[index] = Value.FromNumber(newValue);

                        // Depending on whether it's prefix or postfix, return the new or old value
                        return node.Fixity == OperatorFixity.Prefix ? 
                            Value.FromNumber(newValue) : Value.FromNumber(currentValue);
                    }

                    throw new Exception($"The operand of a {fixity} {op} operator must be a variable or a list element.");


                default:
                    throw new InvalidOperationException($"Unsupported {fixity} operator {op}");
            }
        }

        public void Visit(ExpressionStatement node) => node.Expression.Accept(this);
    }
}