namespace Betty.Core.Interpreter
{
    public class ScopeManager
    {
        private readonly Dictionary<string, Value> _globals = new();
        private readonly Stack<Dictionary<string, Value>> _scopes = new();

        public bool IsGlobalScope => _scopes.Count == 0;

        public void EnterScope()
        {
            _scopes.Push([]);
        }

        public void ExitScope()
        {
            _scopes.Pop();
        }

        public void DeclareGlobal(string name, Value value)
        {
            if (_globals.ContainsKey(name))
            {
                throw new Exception($"Global variable '{name}' is already defined.");
            }

            // Globals are declared directly in the global scope dictionary
            _globals[name] = value;
        }

        public void SetVariable(string name, Value value, bool isFunctionParam = false)
        {
            // Check all active scopes from innermost to outermost
            foreach (var scope in _scopes)
            {
                if (scope.ContainsKey(name))
                {
                    // Update the variable in the first scope where it is found
                    scope[name] = value;
                    return;
                }
            }

            // If the variable is not a function parameter and is not found in any scope, check the globals
            if (!isFunctionParam && _globals.ContainsKey(name))
            {
                // Update the global variable if it exists
                _globals[name] = value;
                return;
            }

            // Otherwise, declare the variable in the current scope
            if (_scopes.Count > 0)
            {
                // We're in a local scope - add to innermost scope
                var currentScope = _scopes.Peek();
                currentScope[name] = value;
            }
            else
            {
                // We're at global level - add to globals
                _globals[name] = value;
            }
        }

        public Value LookupVariable(string name)
        {
            // Check local scopes first
            foreach (var scope in _scopes)
            {
                if (scope.TryGetValue(name, out var value))
                {
                    return value;
                }
            }

            // Then check the global scope
            if (_globals.TryGetValue(name, out var globalValue))
            {
                return globalValue;
            }

            throw new Exception($"Variable '{name}' is not defined in any (active) scope.");
        }

        // Captures all currently accessible variables
        public Dictionary<string, Value> GetAllVariables()
        {
            var result = new Dictionary<string, Value>(_globals);
            // Add local scopes from outermost to innermost
            // so inner scopes override outer ones
            foreach (var scope in _scopes)
            {
                foreach (var kvp in scope)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            return result;
        }

        // Captures only local variables (excludes globals)
        public Dictionary<string, Value>? GetLocalVariables()
        {
            if (_scopes.Count == 0)
                return null; // No local scope to capture

            var result = new Dictionary<string, Value>();
            foreach (var scope in _scopes)
            {
                foreach (var kvp in scope)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            return result;
        }
    }
}