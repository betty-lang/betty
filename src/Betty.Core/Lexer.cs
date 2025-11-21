namespace Betty.Core
{
    public class Lexer
    {
        private readonly string _input;
        private int _position;
        private int _currentLine = 1;
        private int _currentColumn = 1;
        private char _currentChar;
        private readonly System.Text.StringBuilder _stringBuilder = new();

        private static readonly Dictionary<string, TokenType> _reservedKeywords = new()
        {
            ["func"] = TokenType.Func,
            ["global"] = TokenType.Global,
            ["true"] = TokenType.TrueKeyword,
            ["false"] = TokenType.FalseKeyword,
            ["if"] = TokenType.If,
            ["then"] = TokenType.Then,
            ["elif"] = TokenType.Elif,
            ["else"] = TokenType.Else,
            ["for"] = TokenType.For,
            ["foreach"] = TokenType.ForEach,
            ["in"] = TokenType.In,
            ["while"] = TokenType.While,
            ["do"] = TokenType.Do,
            ["break"] = TokenType.Break,
            ["continue"] = TokenType.Continue,
            ["return"] = TokenType.Return,
            ["switch"] = TokenType.Switch,
            ["case"] = TokenType.Case,
            ["default"] = TokenType.Default,
            ["_"] = TokenType.Underscore
        };

        private static readonly Dictionary<char, TokenType> _singleCharOperators = new()
        {
            ['+'] = TokenType.Plus,
            ['-'] = TokenType.Minus,
            ['*'] = TokenType.Mul,
            ['/'] = TokenType.Div,
            ['^'] = TokenType.Caret,
            ['('] = TokenType.LParen,
            [')'] = TokenType.RParen,
            ['{'] = TokenType.LBrace,
            ['}'] = TokenType.RBrace,
            [';'] = TokenType.Semicolon,
            ['!'] = TokenType.Not,
            ['='] = TokenType.Equal,
            ['<'] = TokenType.LessThan,
            ['>'] = TokenType.GreaterThan,
            [','] = TokenType.Comma,
            ['?'] = TokenType.QuestionMark,
            [':'] = TokenType.Colon,
            ['%'] = TokenType.Mod,
            ['['] = TokenType.LBracket,
            [']'] = TokenType.RBracket
        };

        private static readonly Dictionary<string, TokenType> _multiCharOperators = new()
        {
            [".."] = TokenType.DotDot, // Range operator [start..end]
            ["=="] = TokenType.EqualEqual,
            ["<="] = TokenType.LessThanOrEqual,
            [">="] = TokenType.GreaterThanOrEqual,
            ["!="] = TokenType.NotEqual,
            ["&&"] = TokenType.And,
            ["||"] = TokenType.Or,
            ["++"] = TokenType.Increment,
            ["--"] = TokenType.Decrement,
            ["+="] = TokenType.PlusEqual,
            ["-="] = TokenType.MinusEqual,
            ["*="] = TokenType.MulEqual,
            ["/="] = TokenType.DivEqual,
            ["^="] = TokenType.CaretEqual,
            ["%="] = TokenType.ModEqual,
            ["//"] = TokenType.IntDiv,
            ["//="] = TokenType.IntDivEqual,
            ["=>"] = TokenType.Arrow
        };

        public Lexer(string input)
        {
            _input = input;
            _position = 0;
            _currentChar = _input.Length > 0 ? _input[_position] : '\0'; // Handle empty input
        }

        private void Advance(int offset = 1)
        {
            for (int i = 0; i < offset; i++)
            {
                if (_currentChar == '\n')
                {
                    _currentLine++;
                    _currentColumn = 1;
                }
                else
                {
                    _currentColumn++;
                }

                _position++;
                if (_position >= _input.Length)
                {
                    _currentChar = '\0';
                    break; // Exit the loop if we've reached the end of the input
                }
                else
                {
                    _currentChar = _input[_position];
                }
            }
        }

        private void SkipWhitespace()
        {
            while (_currentChar != '\0' && Char.IsWhiteSpace(_currentChar))
                Advance();
        }

        private Token ScanStringLiteral()
        {
            _stringBuilder.Clear();
            int startLine = _currentLine;
            int startColumn = _currentColumn;

            Advance(); // Skip the opening quote

            while (_currentChar != '"')
            {
                if (_currentChar == '\\') // Check for escape character
                {
                    Advance(); // Skip the escape character
                    switch (_currentChar)
                    {
                        case 'n': _stringBuilder.Append('\n'); break; // Newline
                        case 't': _stringBuilder.Append('\t'); break; // Tab
                        case '"': _stringBuilder.Append('\"'); break; // Double quote
                        case '\'': _stringBuilder.Append('\''); break; // Single quote
                        case '\\': _stringBuilder.Append('\\'); break; // Backslash
                        case '0': _stringBuilder.Append('\0'); break; // Null character
                        default:
                            return new Token(TokenType.Error, $"Unrecognized escape sequence: \\{_currentChar}", startLine, startColumn);
                    }
                    Advance(); // Move past the character after the escape
                }
                else
                {
                    _stringBuilder.Append(_currentChar);
                    Advance();
                }

                if (_currentChar == '\0')
                    return new Token(TokenType.Error, "Unterminated string literal.", startLine, startColumn);
            }

            Advance(); // Skip the closing quote
            return new Token(TokenType.StringLiteral, _stringBuilder.ToString(), startLine, startColumn);
        }

        private Token ScanNumberLiteral(bool hasLeadingDot, int startLine, int startColumn)
        {
            _stringBuilder.Clear();

            bool dotEncountered = hasLeadingDot;

            if (hasLeadingDot)
            {
                _stringBuilder.Append("0.");
                Advance(); // Move past the dot character
            }

            while (Char.IsDigit(_currentChar) || _currentChar == '.')
            {
                // Break if the next character is also a dot (range operator)
                if (_currentChar == '.' && Peek() == '.')
                    break;

                if (_currentChar == '.')
                {
                    if (dotEncountered) // Return error token when encountering multiple dots
                        return new Token(TokenType.Error, "Invalid numeric format with multiple dots.", startLine, startColumn);

                    dotEncountered = true;
                }

                _stringBuilder.Append(_currentChar);
                Advance();
            }

            // Use invariant culture to parse numbers to ensure consistent behavior across different locales
            if (double.TryParse(_stringBuilder.ToString(), System.Globalization.CultureInfo.InvariantCulture, out double result))
                return new Token(TokenType.NumberLiteral, result, startLine, startColumn);
            else
                return new Token(TokenType.Error, $"Invalid number format: {_stringBuilder}", startLine, startColumn);
        }

        private Token ScanIdentifierOrKeyword()
        {
            _stringBuilder.Clear();

            while (_currentChar != '\0' && (Char.IsLetterOrDigit(_currentChar) || _currentChar == '_'))
            {
                _stringBuilder.Append(_currentChar);
                Advance();
            }

            var result = _stringBuilder.ToString().ToLower();

            if (_reservedKeywords.TryGetValue(result, out TokenType type))
                return new Token(type, _currentLine, _currentColumn);  

            return new Token(TokenType.Identifier, result, _currentLine, _currentColumn);
        }

        private char Peek(int lookahead = 1)
        {
            int offset = _position + lookahead;
            if (offset >= _input.Length)
            {
                return '\0'; // Return null character if peeking past the end of input
            }
            else
            {
                return _input[offset];
            }
        }

        private void SkipComment()
        {
            while (_currentChar != '\0' && _currentChar != '\n')
                Advance();
        }

        public Token PeekNextToken()
        {
            // Save the current state
            var currentPosition = _position;
            var currentChar = _currentChar;

            var nextToken = GetNextToken();

            // Restore the saved state
            _position = currentPosition;
            _currentChar = currentChar;

            return nextToken;
        }

        private Token ScanCharLiteral(int startLine, int startColumn)
        {
            Advance(); // Skip the opening quote

            var charLiteral = _currentChar;

            // Check for escape character
            bool isEscapeChar = charLiteral == '\\';
            if (isEscapeChar)
            {
                Advance(); // Skip the escape character

                // Replace the escape sequence with the actual character
                charLiteral = _currentChar switch
                {
                    'n' => '\n',    // Newline
                    't' => '\t',    // Tab
                    '"' => '\"',    // Double quote
                    '\'' => '\'',   // Single quote
                    '\\' => '\\',   // Backslash
                    '0' => '\0',    // Null character
                    _ => '\0'       // Placeholder for error case
                };

                if (_currentChar != 'n' && _currentChar != 't' && _currentChar != '"' &&
                    _currentChar != '\'' && _currentChar != '\\' && _currentChar != '0')
                {
                    return new Token(TokenType.Error, $"Unrecognized escape sequence: \\{_currentChar}", startLine, startColumn);
                }
            }

            if (!isEscapeChar && _currentChar == '\'') // Check for empty character literal
                return new Token(TokenType.Error, "Empty character literal.", startLine, startColumn);

            Advance(); // Move past the character

            if (_currentChar != '\'') // Check for unterminated character literal
                return new Token(TokenType.Error, "Unterminated character literal.", startLine, startColumn);

            Advance(); // Skip the closing quote
            return new Token(TokenType.CharLiteral, charLiteral, startLine, startColumn);
        }

        private Token ScanOperator(int startLine, int startColumn)
        {
            // Start by building a two-character operator
            string multiCharOperator = _currentChar.ToString() + Peek();

            // Check if the two-character sequence is a valid operator
            if (_multiCharOperators.TryGetValue(multiCharOperator, out TokenType type))
            {
                var twoCharTokenType = type;

                // Peek ahead one more character to see if there's a valid three-character operator
                multiCharOperator += Peek(2); // Peek two characters ahead

                // Check if the three-character sequence is a valid operator
                if (_multiCharOperators.TryGetValue(multiCharOperator, out type))
                {
                    Advance(3); // Move past the three-character operator
                    return new Token(type, null, startLine, startColumn);
                }
                else
                {
                    Advance(2); // Move past the two-character operator if no valid three-character operator found
                    return new Token(twoCharTokenType, null, startLine, startColumn);
                }
            }

            // If we reach here, no valid two or three-character operator was found; handle as a single character

            if (_singleCharOperators.TryGetValue(_currentChar, out type))
            {
                Advance(); // Move past the single character operator
                return new Token(type, null, startLine, startColumn);
            }

            // Unrecognized character - return error token and advance
            char errorChar = _currentChar;
            Advance();
            return new Token(TokenType.Error, $"Unrecognized character: '{errorChar}'", startLine, startColumn);
        }

        public Token GetNextToken()
        {
            while (_currentChar != '\0')
            {
                int startLine = _currentLine;
                int startColumn = _currentColumn;

                if (Char.IsWhiteSpace(_currentChar))
                {
                    SkipWhitespace();
                    continue;
                }

                if (_currentChar == '#')
                {
                    SkipComment();
                    continue;
                }

                if (Char.IsLetter(_currentChar) || _currentChar == '_')
                    return ScanIdentifierOrKeyword();

                if (Char.IsDigit(_currentChar))
                    return ScanNumberLiteral(hasLeadingDot: false, startLine, startColumn);

                if (_currentChar == '.' && Char.IsDigit(Peek()))
                    return ScanNumberLiteral(hasLeadingDot: true, startLine, startColumn);

                if (_currentChar == '\'')
                    return ScanCharLiteral(startLine, startColumn);

                if (_currentChar == '"')
                    return ScanStringLiteral();

                return ScanOperator(startLine, startColumn);
            }

            return new Token(TokenType.EOF);
        }
    }
}