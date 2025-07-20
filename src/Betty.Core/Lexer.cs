using Betty.Core.Errors;
using System.Globalization;
using System.Text;

namespace Betty.Core;

public class Lexer(string input)
{
    private readonly string _input = input;
    private int _position;
    private int _currentLine = 1;
    private int _currentColumn = 1;
    private char _currentChar = input.Length > 0 ? input[0] : '\0';
    private readonly StringBuilder _stringBuilder = new();
    public List<Error> Errors { get; } = [];

    private static readonly Dictionary<string, TokenType> ReservedKeywords = new()
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

    private static readonly Dictionary<char, TokenType> SingleCharOperators = new()
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

    private static readonly Dictionary<string, TokenType> MultiCharOperators = new()
    {
        [".." ] = TokenType.DotDot, // Range operator [start..end]
        ["==" ] = TokenType.EqualEqual,
        ["<=" ] = TokenType.LessThanOrEqual,
        [">=" ] = TokenType.GreaterThanOrEqual,
        ["!=" ] = TokenType.NotEqual,
        ["&&" ] = TokenType.And,
        ["||" ] = TokenType.Or,
        ["++" ] = TokenType.Increment,
        ["--" ] = TokenType.Decrement,
        ["+=" ] = TokenType.PlusEqual,
        ["-=" ] = TokenType.MinusEqual,
        ["*=" ] = TokenType.MulEqual,
        ["/=" ] = TokenType.DivEqual,
        ["^=" ] = TokenType.CaretEqual,
        ["%=" ] = TokenType.ModEqual,
        ["//" ] = TokenType.IntDiv,
        ["//=" ] = TokenType.IntDivEqual
    };

    private void Advance(int offset = 1)
    {
        for (var i = 0; i < offset; i++)
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
            _currentChar = _position < _input.Length ? _input[_position] : '\0';
        }
    }

    private void SkipWhitespace()
    {
        while (_currentChar != '\0' && char.IsWhiteSpace(_currentChar))
        {
            Advance();
        }
    }

    private void SkipComment()
    {
        while (_currentChar != '\0' && _currentChar != '\n')
        {
            Advance();
        }
    }

    private char Peek(int lookahead = 1)
    {
        var peekPosition = _position + lookahead;
        return peekPosition < _input.Length ? _input[peekPosition] : '\0';
    }

    public Token GetNextToken()
    {
        while (_currentChar != '\0')
        {
            if (char.IsWhiteSpace(_currentChar))
            {
                SkipWhitespace();
                continue;
            }

            if (_currentChar == '#')
            {
                SkipComment();
                continue;
            }

            if (char.IsLetter(_currentChar) || _currentChar == '_')
            {
                return ScanIdentifierOrKeyword();
            }

            if (char.IsDigit(_currentChar))
            {
                return ScanNumberLiteral(false);
            }

            if (_currentChar == '.' && char.IsDigit(Peek()))
            {
                return ScanNumberLiteral(true);
            }

            return _currentChar switch
            {
                '\'' => ScanCharLiteral(),
                '"' => ScanStringLiteral(),
                _ => ScanOperator()
            };
        }

        return new Token(TokenType.EOF);
    }

    public List<Token> GetTokens()
    {
        var tokens = new List<Token>();
        Token token;
        do
        {
            token = GetNextToken();
            tokens.Add(token);
        } while (token.Type != TokenType.EOF);
        return tokens;
    }

    public Token PeekNextToken()
    {
        // Save the current state
        var savedPosition = _position;
        var savedChar = _currentChar;
        var savedLine = _currentLine;
        var savedColumn = _currentColumn;
        var savedErrors = new List<Error>(Errors);

        var nextToken = GetNextToken();

        // Restore the saved state
        _position = savedPosition;
        _currentChar = savedChar;
        _currentLine = savedLine;
        _currentColumn = savedColumn;
        Errors.Clear();
        Errors.AddRange(savedErrors);

        return nextToken;
    }

    private Token ScanIdentifierOrKeyword()
    {
        _stringBuilder.Clear();

        while (_currentChar != '\0' && (char.IsLetterOrDigit(_currentChar) || _currentChar == '_'))
        {
            _stringBuilder.Append(_currentChar);
            Advance();
        }

        var identifier = _stringBuilder.ToString();
        if (ReservedKeywords.TryGetValue(identifier.ToLower(), out var type))
        {
            return new Token(type, line: _currentLine, column: _currentColumn);
        }

        return new Token(TokenType.Identifier, identifier, _currentLine, _currentColumn);
    }

    private Token ScanNumberLiteral(bool hasLeadingDot)
    {
        var startColumn = _currentColumn;
        _stringBuilder.Clear();
        var dotEncountered = hasLeadingDot;

        if (hasLeadingDot)
        {
            _stringBuilder.Append("0.");
            Advance();
        }

        while (char.IsDigit(_currentChar) || _currentChar == '.')
        {
            if (_currentChar == '.' && Peek() == '.')
            {
                break;
            }

            if (_currentChar == '.')
            {
                if (dotEncountered)
                {
                    var err = new Error("Invalid numeric format with multiple dots.", _currentLine, _currentColumn);
                    Errors.Add(err);
                    return new Token(TokenType.Error, err.Message, _currentLine, startColumn);
                }
                dotEncountered = true;
            }

            _stringBuilder.Append(_currentChar);
            Advance();
        }

        var value = double.Parse(_stringBuilder.ToString(), CultureInfo.InvariantCulture);
        return new Token(TokenType.NumberLiteral, value, _currentLine, startColumn);
    }

    private Token ScanStringLiteral()
    {
        var startColumn = _currentColumn;
        _stringBuilder.Clear();
        Advance(); // Skip opening quote

        while (_currentChar != '"')
        {
            if (_currentChar == '\0')
            {
                var err = new Error("Unterminated string literal.", _currentLine, _currentColumn);
                Errors.Add(err);
                return new Token(TokenType.Error, err.Message, _currentLine, startColumn);
            }

            if (_currentChar == '\\')
            {
                Advance(); // Skip escape char
                var (esc, error) = ScanEscapeSequence();
                if (error != null)
                {
                    Errors.Add(error);
                    return new Token(TokenType.Error, error.Message, _currentLine, startColumn);
                }
                _stringBuilder.Append(esc);
            }
            else
            {
                _stringBuilder.Append(_currentChar);
            }
            Advance();
        }

        Advance(); // Skip closing quote
        return new Token(TokenType.StringLiteral, _stringBuilder.ToString(), _currentLine, startColumn);
    }

    private Token ScanCharLiteral()
    {
        var startColumn = _currentColumn;
        Advance(); // Skip opening quote

        if (_currentChar == '\'')
        {
            var err = new Error("Empty character literal.", _currentLine, _currentColumn);
            Errors.Add(err);
            return new Token(TokenType.Error, err.Message, _currentLine, startColumn);
        }

        char value;
        if (_currentChar == '\\')
        {
            Advance(); // Skip escape char
            var (esc, error) = ScanEscapeSequence();
            if (error != null)
            {
                Errors.Add(error);
                return new Token(TokenType.Error, error.Message, _currentLine, startColumn);
            }
            value = esc;
        }
        else
        {
            value = _currentChar;
        }

    Advance();

        if (_currentChar != '\'')
        {
            var err = new Error("Unterminated character literal.", _currentLine, _currentColumn);
            Errors.Add(err);
            return new Token(TokenType.Error, err.Message, _currentLine, startColumn);
        }

        Advance(); // Skip closing quote
        return new Token(TokenType.CharLiteral, value, _currentLine, startColumn);
    }

    private (char, Error?) ScanEscapeSequence() => _currentChar switch
    {
        'n' => ('\n', null),
        't' => ('\t', null),
        '"' => ('"', null),
        '\'' => ('\'', null),
        '\\' => ('\\', null),
        '0' => ('\0', null),
        _ => ('\0', new Error($"Unrecognized escape sequence: \\{_currentChar}", _currentLine, _currentColumn))
    };

    private Token ScanOperator()
    {
        var startColumn = _currentColumn;
        // Greedily check for the longest possible operator
        var op3 = _stringBuilder.Clear().Append(_currentChar).Append(Peek()).Append(Peek(2)).ToString();
        if (MultiCharOperators.TryGetValue(op3, out var tokenType))
        {
            Advance(3);
            return new Token(tokenType, null, _currentLine, startColumn);
        }

        var op2 = op3.Substring(0, 2);
        if (MultiCharOperators.TryGetValue(op2, out tokenType))
        {
            Advance(2);
            return new Token(tokenType, null, _currentLine, startColumn);
        }

        if (SingleCharOperators.TryGetValue(_currentChar, out tokenType))
        {
            Advance();
            return new Token(tokenType, null, _currentLine, startColumn);
        }

        var err = new Error($"Unrecognized character: {_currentChar}", _currentLine, _currentColumn);
        Errors.Add(err);
        Advance();
        return new Token(TokenType.Error, err.Message, _currentLine, startColumn);
    }
}