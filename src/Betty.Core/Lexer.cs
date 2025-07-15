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
            ["true"] = TokenType.True,
            ["false"] = TokenType.False,
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
            ["return"] = TokenType.Return
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
            ["//="] = TokenType.IntDivEqual
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

        private string ScanStringLiteral()
        {
            _stringBuilder.Clear();

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
                            throw new Exception($"Unrecognized escape sequence: \\{_currentChar}");
                    }
                    Advance(); // Move past the character after the escape
                }
                else
                {
                    _stringBuilder.Append(_currentChar);
                    Advance();
                }

                if (_currentChar == '\0')
                    throw new Exception("Unterminated string literal.");
            }

            Advance(); // Skip the closing quote
            return _stringBuilder.ToString();
        }

        private double ScanNumberLiteral(bool hasLeadingDot)
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
                    if (dotEncountered) // Throw when encountering multiple dots
                        throw new FormatException("Invalid numeric format with multiple dots.");

                    dotEncountered = true;
                }

                _stringBuilder.Append(_currentChar);
                Advance();
            }

            // Use invariant culture to parse numbers to ensure consistent behavior across different locales
            return double.Parse(_stringBuilder.ToString(), System.Globalization.CultureInfo.InvariantCulture);
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

        private char ScanCharLiteral()
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
                    _ => throw new Exception($"Unrecognized escape sequence: \\{_currentChar}"),
                };
            }

            if (!isEscapeChar && _currentChar == '\'') // Check for empty character literal
                throw new Exception("Empty character literal.");

            Advance(); // Move past the character

            if (_currentChar != '\'') // Check for unterminated character literal
                throw new Exception("Unterminated character literal.");
            Advance(); // Skip the closing quote
            return charLiteral;
        }

        private Token ScanOperator()
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
                    return new Token(type, null, _currentLine, _currentColumn);
                }
                else
                {
                    Advance(2); // Move past the two-character operator if no valid three-character operator found
                    return new Token(twoCharTokenType, null, _currentLine, _currentColumn);
                }
            }

            // If we reach here, no valid two or three-character operator was found; handle as a single character

            if (_singleCharOperators.TryGetValue(_currentChar, out type))
            {
                Advance(); // Move past the single character operator
                return new Token(type, null, _currentLine, _currentColumn);
            }

            throw new Exception($"Unrecognized character: {_currentChar} at line {_currentLine}, column {_currentColumn}");
        }

        public Token GetNextToken()
        {
            while (_currentChar != '\0')
            {
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
                    return new Token(TokenType.NumberLiteral, ScanNumberLiteral(hasLeadingDot: false), _currentLine, _currentColumn);

                if (_currentChar == '.' && Char.IsDigit(Peek()))
                    return new Token(TokenType.NumberLiteral, ScanNumberLiteral(hasLeadingDot: true), _currentLine, _currentColumn);

                if (_currentChar == '\'')
                    return new Token(TokenType.CharLiteral, ScanCharLiteral(), _currentLine, _currentColumn);

                if (_currentChar == '"')
                    return new Token(TokenType.StringLiteral, ScanStringLiteral(), _currentLine, _currentColumn);

                return ScanOperator(); // This will throw if the character is not a valid operator
            }

            return new Token(TokenType.EOF);
        }
    }
}