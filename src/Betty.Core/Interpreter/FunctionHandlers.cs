using Betty.Core.AST;
using System.Collections.Generic;

namespace Betty.Core.Interpreter
{
    public partial class Interpreter
    {
        public Value Visit(FunctionDefinition node)
        {
            if (_intrinsicFunctions.ContainsKey(node.FunctionName!))
                throw new Exception($"Function name '{node.FunctionName}' is reserved for built-in functions.");

            _functions[node.FunctionName!] = node;
            return Value.None();
        }

        public Value Visit(FunctionCall node)
        {
            // First, check if it's an intrinsic function.
            if (node.FunctionName is not null && _intrinsicFunctions.TryGetValue(node.FunctionName, out var intrinsicFunction))
            {
                return intrinsicFunction.Execute(this, node);
            }

            // If not intrinsic, resolve it as a user-defined function.
            var function = ResolveFunction(node);
            if (function is null)
            {
                throw new Exception($"Function not found: {node.FunctionName ?? "anonymous function"}");
            }

            // Execute the user-defined function.
            return ExecuteUserFunction(function, node.Arguments);
        }

        private FunctionDefinition? ResolveFunction(FunctionCall node)
        {
            // Case 1: User-defined function by name (e.g., my_func())
            if (node.FunctionName is not null && _functions.TryGetValue(node.FunctionName, out var globalFunc))
            {
                return globalFunc;
            }

            // Case 2: First-class function from an expression (e.g., (func() { ... })())
            if (node.Expression is not null)
            {
                var functionValue = node.Expression.Accept(this);
                if (functionValue?.Type == ValueType.Function)
                {
                    var funcExpr = functionValue.AsFunction();
                    return new FunctionDefinition(null, funcExpr.Parameters, funcExpr.Body);
                }
            }

            // Case 3: Function name is a variable
            if (node.FunctionName is not null)
            {
                var funcFromVar = _scopeManager.LookupVariable(node.FunctionName);
                if (funcFromVar.Type == ValueType.Function)
                {
                    var funcExpr = funcFromVar.AsFunction();
                    return new FunctionDefinition(null, funcExpr.Parameters, funcExpr.Body);
                }
            }

            return null;
        }

        private Value ExecuteUserFunction(FunctionDefinition function, List<Expression> arguments)
        {
            _scopeManager.EnterScope();
            var previousContext = _context.EnterFunction();

            if (function.Parameters.Count != arguments.Count)
                throw new Exception($"Expected {function.Parameters.Count} arguments but got {arguments.Count}.");

            for (int i = 0; i < arguments.Count; i++)
            {
                var argValue = arguments[i].Accept(this);
                _scopeManager.SetVariable(function.Parameters[i], argValue, true);
            }

            function.Body.Accept(this);

            Value returnValue = _context.GetReturnValue();
            _context.Restore(previousContext);
            _scopeManager.ExitScope();

            return returnValue;
        }
    }
}
