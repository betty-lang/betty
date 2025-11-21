using Betty.Core.AST;
using Expression = Betty.Core.AST.Expression;

namespace Betty.Core
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _currentToken;
        private readonly List<string> _errors = new();

        // Synchronizing tokens for Panic Mode recovery
        private static readonly HashSet<TokenType> _synchronizingTokens = new()
        {
            TokenType.Semicolon,
            TokenType.RBrace,
            TokenType.If,
            TokenType.While,
            TokenType.For,
            TokenType.ForEach,
            TokenType.Do,
            TokenType.Switch,
            TokenType.Return,
            TokenType.Break,
            TokenType.Continue,
            TokenType.Func,
        };

        public IReadOnlyList<string> Errors => _errors;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = _lexer.GetNextToken(); // Set current token to the first token from the input
        }

        private static readonly HashSet<TokenType> _assignmentOperators = new()
        {
            TokenType.Equal,
            TokenType.PlusEqual,
            TokenType.MinusEqual,
            TokenType.MulEqual,
            TokenType.DivEqual,
            TokenType.ModEqual,
            TokenType.IntDivEqual,
            TokenType.CaretEqual,
        };

        private int GetPrecedence()
        {
            // Define operator precedences
            return _currentToken.Type switch
            {
                TokenType.QuestionMark => 1,  // Ternary operator precedence

                // Comparison operators
                TokenType.EqualEqual => 2,
                TokenType.NotEqual => 2,
                TokenType.LessThan => 2,
                TokenType.LessThanOrEqual => 2,
                TokenType.GreaterThan => 2,
                TokenType.GreaterThanOrEqual => 2,

                // Logical operators
                TokenType.And => 3,
                TokenType.Or => 4,

                // Arithmetic operators
                TokenType.Plus => 5,
                TokenType.Minus => 5,

                // Multiplicative operators
                TokenType.Mul => 6,
                TokenType.Div => 6,
                TokenType.Mod => 6,
                TokenType.IntDiv => 6,

                // Exponentiation (highest precedence)
                TokenType.Caret => 7,

                _ => 0,  // Default precedence for unknown operators
            };
        }

        private void ReportError(string message)
        {
            _errors.Add($"[Line {_currentToken.Line}, Column {_currentToken.Column}] {message}");
        }

        private void Consume(TokenType tokenType)
        {
            // Check for lexer errors first
            if (_currentToken.Type == TokenType.Error)
            {
                ReportError((string)_currentToken.Value!);
                _currentToken = _lexer.GetNextToken();
                return;
            }

            if (_currentToken.Type == tokenType)
            {
                _currentToken = _lexer.GetNextToken();
            }
            else
            {
                ReportError($"Expected {tokenType}, found {_currentToken.Type}");
            }
        }

        private void Synchronize()
        {
            // Skip tokens until we find a synchronizing point
            while (_currentToken.Type != TokenType.EOF &&
                   !_synchronizingTokens.Contains(_currentToken.Type))
            {
                _currentToken = _lexer.GetNextToken();
            }

            // If we stopped at a semicolon, consume it to move past it
            if (_currentToken.Type == TokenType.Semicolon)
            {
                _currentToken = _lexer.GetNextToken();
            }
        }

        private void SynchronizeToClosingBrace()
        {
            int braceDepth = 1;
            while (_currentToken.Type != TokenType.EOF && braceDepth > 0)
            {
                if (_currentToken.Type == TokenType.LBrace)
                    braceDepth++;
                else if (_currentToken.Type == TokenType.RBrace)
                    braceDepth--;

                if (braceDepth > 0)
                    _currentToken = _lexer.GetNextToken();
            }
        }

        private Expression ParseExpression(int precedence = 0)
        {
            // Handle error tokens from lexer before parsing
            if (_currentToken.Type == TokenType.Error)
            {
                var errorToken = _currentToken;
                ReportError((string)errorToken.Value!);
                _currentToken = _lexer.GetNextToken();
                return new ErrorExpression((string)errorToken.Value!, errorToken.Line, errorToken.Column);
            }

            var left = ParsePrimary();

            while (true)
            {
                var currentPrecedence = GetPrecedence();

                // Check if the current operator's precedence allows us to continue parsing
                if (precedence > currentPrecedence)
                    break;

                var token = _currentToken;

                // Special handling for assignment (right-associative)
                if (_assignmentOperators.Contains(token.Type))
                {
                    // Only parse assignment if we're at a precedence level that allows it
                    if (precedence > 0)
                        break;

                    Consume(token.Type);
                    var right = ParseExpression(0);  // Parse the right side with minimum precedence
                    left = new AssignmentExpression(left, right, token.Type);
                }
                else if (token.Type == TokenType.QuestionMark)
                {
                    // Ternary operator handling (right-associative)
                    Consume(TokenType.QuestionMark);

                    // Parse the true expression part of the ternary operator
                    var trueExpression = ParseExpression(0);  // Ternary is right-associative, so parse with min precedence

                    Consume(TokenType.Colon);

                    // Parse the false expression part of the ternary operator
                    // IMPORTANT: Parse false part with the SAME precedence to ensure right-associativity
                    var falseExpression = ParseExpression(currentPrecedence);

                    // Create the ternary expression node
                    left = new TernaryOperatorExpression(left, trueExpression, falseExpression);
                }
                else
                {
                    // For other operators (left-associative)
                    if (precedence >= currentPrecedence)
                        break;

                    Consume(token.Type);
                    var right = ParseExpression(currentPrecedence + (token.Type == TokenType.Caret ? 1 : 0));
                    left = new BinaryOperatorExpression(left, token.Type, right);
                }
            }

            return left;
        }

        private FunctionCall ParseRange()
        {
            var start = ParseExpression(); // Parse the start of the range
            Consume(TokenType.DotDot);
            var end = ParseExpression(); // Parse the end of the range
            Consume(TokenType.RBracket); // Consume the closing bracket (end of the range)

            // Construct and return a FunctionCall AST node for the range function
            return new FunctionCall(new List<Expression> { start, end }, functionName : "range");
        }

        private Expression ParseListOrRange()
        {
            Consume(TokenType.LBracket); // Consume the opening bracket

            // Check if the list is actually a range
            if (_lexer.PeekNextToken().Type == TokenType.DotDot)
                return ParseRange();

            var elements = new List<Expression>(); // Create a list to store the elements

            // Check if the list is not empty
            if (_currentToken.Type != TokenType.RBracket)
            {
                do
                {
                    if (_currentToken.Type == TokenType.Comma)
                        Consume(TokenType.Comma);

                    elements.Add(ParseExpression());
                }
                while (_currentToken.Type == TokenType.Comma);
            }

            Consume(TokenType.RBracket); // Consume the closing bracket
            return new ListLiteral(elements);
        }

        private IndexerExpression ParseIndexerExpression(Expression listExpr)
        {
            Consume(TokenType.LBracket);
            var indexExpr = ParseExpression();
            Consume(TokenType.RBracket);
            return new IndexerExpression(listExpr, indexExpr);
        }

        private Expression ParsePrimary()
        {
            // Step 1: Handle primary expressions and unary operators
            Expression expr = ParsePrefix();

            // Step 2: Loop to handle postfix operations (function calls, indexers, postfix operators, switch expressions)
            while (true)
            {
                switch (_currentToken.Type)
                {
                    case TokenType.LParen:
                        // Function call
                        expr = ParseFunctionCall(expr);
                        break;

                    case TokenType.LBracket:
                        // Indexer expression
                        expr = ParseIndexerExpression(expr);
                        break;

                    case TokenType.Increment:
                    case TokenType.Decrement:
                        // Postfix increment or decrement
                        var operatorToken = _currentToken;
                        Consume(operatorToken.Type);
                        expr = new UnaryOperatorExpression(expr, operatorToken.Type, OperatorFixity.Postfix);
                        break;

                    case TokenType.Switch:
                        // Switch expression (postfix operation)
                        expr = ParseSwitchExpression(expr);
                        break;

                    default:
                        // No postfix operation, return the expression
                        return expr;
                }
            }
        }

        private SwitchExpression ParseSwitchExpression(Expression expr)
        {
            Consume(TokenType.Switch);
            Consume(TokenType.LBrace);
            var cases = new List<SwitchExpressionCase>();
            while (_currentToken.Type != TokenType.RBrace)
            {
                if (_currentToken.Type == TokenType.Underscore)  // Checking for default '_'
                    break;  // Stop parsing regular cases if we encounter '_'

                var condition = ParseExpression();
                Consume(TokenType.Arrow);

                var caseResult = ParseExpression();

                cases.Add(new SwitchExpressionCase(condition, caseResult));

                if (_currentToken.Type == TokenType.Comma)
                {
                    Consume(TokenType.Comma); // Consume comma between cases
                }
            }

            // Handle default case if present
            if (_currentToken.Type == TokenType.Underscore) // Default case indicated by '_'
            {
                Consume(TokenType.Underscore);
                Consume(TokenType.Arrow);
                var defaultResult = ParseExpression();
                cases.Add(new SwitchExpressionCase(null, defaultResult));
            }

            Consume(TokenType.RBrace);
            return new SwitchExpression(expr, cases);
        }

        private Expression ParsePrefix()
        {
            var token = _currentToken;
            Expression expr;

            switch (token.Type)
            {
                // Literals
                case TokenType.NumberLiteral:
                case TokenType.CharLiteral:
                case TokenType.StringLiteral:
                    expr = ParseLiteral(token);
                    Consume(token.Type);
                    break;

                // Boolean keywords
                case TokenType.TrueKeyword:
                case TokenType.FalseKeyword:
                    Consume(token.Type);
                    expr = new BooleanExpression(token.Type == TokenType.TrueKeyword);
                    break;

                // Lists or list ranges
                case TokenType.LBracket:
                    expr = ParseListOrRange();
                    break;

                // Unary operators and prefix increment/decrement
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Not:
                case TokenType.Increment:
                case TokenType.Decrement:
                    Consume(token.Type);
                    expr = new UnaryOperatorExpression(ParsePrimary(), token.Type, OperatorFixity.Prefix);
                    break;

                // Parenthesized expressions
                case TokenType.LParen:
                    Consume(TokenType.LParen);
                    expr = ParseExpression();
                    Consume(TokenType.RParen);
                    break;

                // Variables or function call placeholders
                case TokenType.Identifier:
                    expr = new Variable((string)token.Value!);
                    Consume(TokenType.Identifier);
                    break;

                // Function expression
                case TokenType.Func:
                    expr = ParseFunctionExpression();
                    break;

                // If expression
                case TokenType.If:
                    expr = ParseIfExpression();
                    break;

                // Error token from lexer
                case TokenType.Error:
                    ReportError((string)token.Value!);
                    Consume(TokenType.Error);
                    expr = new ErrorExpression((string)token.Value!, token.Line, token.Column);
                    break;

                default:
                    // Unexpected token - create error node and synchronize
                    ReportError($"Unexpected token in expression: {token.Type}");
                    _currentToken = _lexer.GetNextToken(); // Move past the bad token
                    expr = new ErrorExpression($"Unexpected token: {token.Type}", token.Line, token.Column);
                    break;
            }

            return expr;
        }

        private FunctionExpression ParseFunctionExpression()
        {
            Consume(TokenType.Func);
            Consume(TokenType.LParen);
            var parameters = ParseParameters();
            Consume(TokenType.RParen);

            var body = ParseCompoundStatement();

            return new FunctionExpression(parameters, body);
        }

        private static Expression ParseLiteral(Token token)
        {
            return token.Type switch
            {
                TokenType.NumberLiteral => new NumberLiteral((double)token.Value!),
                TokenType.CharLiteral => new CharLiteral((char)token.Value!),
                TokenType.StringLiteral => new StringLiteral((string)token.Value!),
                _ => throw new InvalidOperationException("Invalid literal type."),
            };
        }

        private CompoundStatement ParseCompoundStatement()
        {
            Consume(TokenType.LBrace);
            var nodes = ParseStatementList();
            Consume(TokenType.RBrace);

            var root = new CompoundStatement();
            foreach (var node in nodes)
                root.Statements.Add(node);

            return root;
        }

        private List<Statement> ParseStatementList()
        {
            var results = new List<Statement>();

            while (_currentToken.Type != TokenType.RBrace && _currentToken.Type != TokenType.EOF)
            {
                var node = ParseStatement();
                results.Add(node);
            }

            return results;
        }

        private IfExpression ParseIfExpression()
        {
            Consume(TokenType.If);
            var condition = ParseExpression();
            Consume(TokenType.Then);
            var thenExpression = ParseExpression();

            var elseIfExpressions = new List<(Expression Condition, Expression Expression)>();
            while (_currentToken.Type == TokenType.Elif)
            {
                Consume(TokenType.Elif);
                var elseIfCondition = ParseExpression();
                Consume(TokenType.Then);
                var elseIfExpression = ParseExpression();
                elseIfExpressions.Add((elseIfCondition, elseIfExpression));
            }
            
            Consume(TokenType.Else);
            var elseExpression = ParseExpression();

            return new IfExpression(condition, thenExpression, elseIfExpressions, elseExpression);
        }

        private IfStatement ParseIfStatement()
        {
            Consume(TokenType.If);
            Consume(TokenType.LParen);
            var condition = ParseExpression();
            Consume(TokenType.RParen);

            // Parse thenStatement as either a compound statement or a single statement
            var thenStatement = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();

            var elseIfStatements = new List<(Expression Condition, Statement Statement)>();
            Statement? elseStatement = null;

            while (_currentToken.Type == TokenType.Elif)
            {
                Consume(TokenType.Elif);
                Consume(TokenType.LParen);
                var elseIfCondition = ParseExpression();
                Consume(TokenType.RParen);
                // Parse elseIfStatement similarly
                var elseIfStatement = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
                elseIfStatements.Add((elseIfCondition, elseIfStatement));
            }

            if (_currentToken.Type == TokenType.Else)
            {
                Consume(TokenType.Else);
                // Parse elseStatement similarly
                elseStatement = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
            }

            return new IfStatement(condition, thenStatement, elseIfStatements, elseStatement);
        }

        private ForStatement ParseForStatement()
        {
            Consume(TokenType.For);
            Consume(TokenType.LParen);

            Expression? initializer = null;
            if (_currentToken.Type != TokenType.Semicolon)
            {
                initializer = ParseExpression();
            }
            Consume(TokenType.Semicolon);

            Expression? condition = null;
            if (_currentToken.Type != TokenType.Semicolon)
            {
                condition = ParseExpression();
            }
            Consume(TokenType.Semicolon);

            Expression? increment = null;
            if (_currentToken.Type != TokenType.RParen)
            {
                increment = ParseExpression();
            }
            Consume(TokenType.RParen);

            var body = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();

            return new ForStatement(initializer, condition, increment, body);
        }

        private WhileStatement ParseWhileStatement()
        {
            Consume(TokenType.While);
            Consume(TokenType.LParen);
            var condition = ParseExpression();
            Consume(TokenType.RParen);
            var body = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
            return new WhileStatement(condition, body);
        }

        private DoWhileStatement ParseDoWhileStatement()
        {             
            Consume(TokenType.Do);
            var body = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
            Consume(TokenType.While);
            Consume(TokenType.LParen);
            var condition = ParseExpression();
            Consume(TokenType.RParen);
            Consume(TokenType.Semicolon);
            return new DoWhileStatement(condition, body);
        }

        private BreakStatement ParseBreakStatement()
        {
            Consume(TokenType.Break);
            Consume(TokenType.Semicolon);
            return new BreakStatement();
        }

        private ContinueStatement ParseContinueStatement()
        {
            Consume(TokenType.Continue);
            Consume(TokenType.Semicolon);
            return new ContinueStatement();
        }

        private ReturnStatement ParseReturnStatement()
        {
            Consume(TokenType.Return);
            Expression? returnValue = null;
            if (_currentToken.Type != TokenType.Semicolon)
            {
                returnValue = ParseExpression();
            }
            Consume(TokenType.Semicolon);
            return new ReturnStatement(returnValue);
        }

        private ForEachStatement ParseForEachStatement()
        {
            Consume(TokenType.ForEach);
            Consume(TokenType.LParen);
            var variableName = (string)_currentToken.Value!;
            Consume(TokenType.Identifier);
            Consume(TokenType.In);
            var listExpression = ParseExpression();
            Consume(TokenType.RParen);
            var body = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
            return new ForEachStatement(variableName, listExpression, body);
        }

        private SwitchStatement ParseSwitchStatement()
        {
            Consume(TokenType.Switch);
            Consume(TokenType.LParen);
            var switchExpression = ParseExpression();
            Consume(TokenType.RParen);
            Consume(TokenType.LBrace);
            var cases = new List<SwitchCase>();
            while (_currentToken.Type == TokenType.Case)
            {
                Consume(TokenType.Case);
                var caseExpression = ParseExpression();
                Consume(TokenType.Colon);
                var caseBody = ParseSwitchCaseBody();
                cases.Add(new SwitchCase(caseExpression, caseBody));
            }
            if (_currentToken.Type == TokenType.Default)
            {
                Consume(TokenType.Default);
                Consume(TokenType.Colon);
                var defaultBody = ParseSwitchCaseBody();
                cases.Add(new SwitchCase(null, defaultBody));
            }
            Consume(TokenType.RBrace);
            return new SwitchStatement(switchExpression, cases);
        }

        private List<Statement> ParseSwitchCaseBody()
        {
            var statements = new List<Statement>();
            while (_currentToken.Type != TokenType.Case &&
                   _currentToken.Type != TokenType.Default &&
                   _currentToken.Type != TokenType.RBrace &&
                   _currentToken.Type != TokenType.EOF)
            {
                statements.Add(ParseStatement());
            }
            return statements;
        }

        private Statement ParseStatement()
        {
            // Handle error tokens from lexer
            if (_currentToken.Type == TokenType.Error)
            {
                var errorToken = _currentToken;
                ReportError((string)errorToken.Value!);
                Consume(TokenType.Error);
                Synchronize(); // Skip to next synchronizing point
                return new ErrorStatement((string)errorToken.Value!, errorToken.Line, errorToken.Column);
            }

            try
            {
                return _currentToken.Type switch
                {
                    TokenType.LBrace => ParseCompoundStatement(),
                    TokenType.If => ParseIfStatement(),
                    TokenType.For => ParseForStatement(),
                    TokenType.ForEach => ParseForEachStatement(),
                    TokenType.While => ParseWhileStatement(),
                    TokenType.Do => ParseDoWhileStatement(),
                    TokenType.Break => ParseBreakStatement(),
                    TokenType.Continue => ParseContinueStatement(),
                    TokenType.Return => ParseReturnStatement(),
                    TokenType.Semicolon => ParseEmptyStatement(),
                    TokenType.Switch => ParseSwitchStatement(),
                    TokenType.Func => ParseFunctionDefinition(),
                    _ => ParseExpressionStatement()
                };
            }
            catch (Exception)
            {
                // If any exception occurs during parsing, create error node and synchronize
                var errorToken = _currentToken;
                ReportError($"Error parsing statement starting with {errorToken.Type}");
                Synchronize();
                return new ErrorStatement($"Error parsing statement", errorToken.Line, errorToken.Column);
            }
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            // Parse an expression generally
            var expression = ParseExpression();

            // Try to consume semicolon, but don't throw if missing
            if (_currentToken.Type != TokenType.Semicolon)
            {
                ReportError($"Expected semicolon after expression, found {_currentToken.Type}");
                // Don't consume - let synchronization handle it
            }
            else
            {
                Consume(TokenType.Semicolon);
            }

            return new ExpressionStatement(expression);
        }

        private EmptyStatement ParseEmptyStatement()
        {
            Consume(TokenType.Semicolon);
            return new EmptyStatement();
        }

        private FunctionCall ParseFunctionCall(Expression functionExpr)
        {
            // Consume the opening parenthesis
            Consume(TokenType.LParen);

            // Parse the arguments
            var arguments = new List<Expression>();
            if (_currentToken.Type != TokenType.RParen) // Check if the next token is not a right parenthesis
            {
                do
                {
                    if (_currentToken.Type == TokenType.Comma)
                        Consume(TokenType.Comma); // Consume the comma before parsing the next argument

                    arguments.Add(ParseExpression());
                }
                while (_currentToken.Type == TokenType.Comma); // Continue if there's a comma (more arguments)
            }

            // Consume the closing parenthesis
            Consume(TokenType.RParen);

            if (functionExpr is Variable identifier)
            {
                return new FunctionCall(arguments, functionName : identifier.Name);
            }

            // Inline anonymous function call
            return new FunctionCall(arguments, functionExpr);
        }

        private FunctionDefinition ParseFunctionDefinition()
        {
            Consume(TokenType.Func);
            string functionName = (string)_currentToken.Value!;
            Consume(TokenType.Identifier); // Function name

            Consume(TokenType.LParen); // Opening parenthesis
            var parameters = ParseParameters();
            Consume(TokenType.RParen); // Closing parenthesis

            var body = ParseCompoundStatement(); // Function body

            return new FunctionDefinition(functionName, parameters, body);
        }

        private List<string> ParseParameters()
        {
            var parameters = new List<string>();

            if (_currentToken.Type != TokenType.RParen) // Check if parameter list is empty
            {
                do
                {
                    if (_currentToken.Type == TokenType.Identifier)
                    {
                        string paramName = (string)_currentToken.Value!;
                        parameters.Add(paramName);
                        Consume(TokenType.Identifier);
                    }
                    else
                    {
                        throw new Exception($"Expected an identifier, found {_currentToken.Type}.");
                    }

                    if (_currentToken.Type == TokenType.Comma)
                    {
                        Consume(TokenType.Comma); // Eat comma and expect another parameter
                    }
                }
                while (_currentToken.Type != TokenType.RParen);
            }

            return parameters;
        }

        private List<string> ParseGlobalVariableDeclarations()
        {
            var globals = new List<string>();

            while (_currentToken.Type == TokenType.Global)
            {
                Consume(TokenType.Global);
                var variableName = (string)_currentToken.Value!;
                Consume(TokenType.Identifier);
                globals.Add(variableName);

                // Check for multiple global variable declarations on the same line
                while (_currentToken.Type == TokenType.Comma)
                {
                    Consume(TokenType.Comma);
                    variableName = (string)_currentToken.Value!;
                    Consume(TokenType.Identifier);
                    globals.Add(variableName);
                }

                Consume(TokenType.Semicolon);
            }

            return globals;
        }

        private Program ParseProgram()
        {
            // Parse global variable declarations at the beginning of the program
            var globals = ParseGlobalVariableDeclarations();

            var functions = new List<FunctionDefinition>();

            while (_currentToken.Type != TokenType.EOF)
            {
                // Handle error tokens
                if (_currentToken.Type == TokenType.Error)
                {
                    ReportError((string)_currentToken.Value!);
                    _currentToken = _lexer.GetNextToken();
                    continue;
                }

                if (_currentToken.Type == TokenType.Func)
                {
                    try
                    {
                        functions.Add(ParseFunctionDefinition());
                    }
                    catch (Exception ex)
                    {
                        ReportError($"Error parsing function: {ex.Message}");
                        SynchronizeToClosingBrace(); // Skip to end of function
                        if (_currentToken.Type == TokenType.RBrace)
                            _currentToken = _lexer.GetNextToken(); // Move past closing brace
                    }
                }
                else
                {
                    ReportError($"Unexpected token at top level: {_currentToken.Type}");
                    _currentToken = _lexer.GetNextToken(); // Skip the bad token
                }
            }

            return new Program(globals, functions);
        }

        public Expression Parse()
        {
            var node = ParseProgram();

            // Don't throw if there are errors - they're already recorded
            // The interpreter can check the Errors property

            return node;
        }
    }
}