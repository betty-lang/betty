using System.Globalization;
using System.Text;

namespace Betty.Core;

public class Lexer
{
    private readonly string _input;
    private int _position;
    private int _currentLine = 1;
    private int _currentColumn = 1;
    private char _currentChar;
    private readonly StringBuilder _stringBuilder = new();

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

    public Lexer(string input)
    {
        _input = input;
        _position = 0;
        _currentChar = _input.Length > 0 ? _input[0] : '\0';
    }

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
                return new Token(TokenType.NumberLiteral, ScanNumberLiteral(false), _currentLine, _currentColumn);
            }

            if (_currentChar == '.' && char.IsDigit(Peek()))
            {
                return new Token(TokenType.NumberLiteral, ScanNumberLiteral(true), _currentLine, _currentColumn);
            }

            return _currentChar switch
            {
                '\'' => new Token(TokenType.CharLiteral, ScanCharLiteral(), _currentLine, _currentColumn),
                '"' => new Token(TokenType.StringLiteral, ScanStringLiteral(), _currentLine, _currentColumn),
                _ => ScanOperator()
            };
        }

        return new Token(TokenType.EOF);
    }

    public Token PeekNextToken()
    {
        // Save the current state
        var savedPosition = _position;
        var savedChar = _currentChar;
        var savedLine = _currentLine;
        var savedColumn = _currentColumn;

        var nextToken = GetNextToken();

        // Restore the saved state
        _position = savedPosition;
        _currentChar = savedChar;
        _currentLine = savedLine;
        _currentColumn = savedColumn;

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

    private double ScanNumberLiteral(bool hasLeadingDot)
    {
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
                    throw new FormatException("Invalid numeric format with multiple dots.");
                }
                dotEncountered = true;
            }

            _stringBuilder.Append(_currentChar);
            Advance();
        }

        return double.Parse(_stringBuilder.ToString(), CultureInfo.InvariantCulture);
    }

    private string ScanStringLiteral()
    {
        _stringBuilder.Clear();
        Advance(); // Skip opening quote

        while (_currentChar != '"')
        {
            if (_currentChar == '\0')
            {
                throw new Exception("Unterminated string literal.");
            }

            if (_currentChar == '\\')
            {
                Advance(); // Skip escape char
                _stringBuilder.Append(ScanEscapeSequence());
            }
            else
            {
                _stringBuilder.Append(_currentChar);
            }
            Advance();
        }

        Advance(); // Skip closing quote
        return _stringBuilder.ToString();
    }

private char ScanCharLiteral()
{
    Advance(); // Skip opening quote

    if (_currentChar == '\'')
    {
        throw new Exception("Empty character literal.");
    }

    char value;
    if (_currentChar == '\\')
    {
        Advance(); // Skip escape char
        value = ScanEscapeSequence();
    }
    else
    {
        value = _currentChar;
    }

    Advance();

    if (_currentChar != '\'')
    {
        throw new Exception("Unterminated character literal.");
    }

    Advance(); // Skip closing quote
    return value;
}

    private char ScanEscapeSequence() => _currentChar switch
    {
        'n' => '\n',
        't' => '\t',
        '"' => '"',
        '\'' => '\'',
        '\\' => '\\',
        '0' => '\0',
        _ => throw new Exception($"Unrecognized escape sequence: \\{_currentChar}")
    };

    private Token ScanOperator()
    {
        // Greedily check for the longest possible operator
        var op3 = _stringBuilder.Clear().Append(_currentChar).Append(Peek()).Append(Peek(2)).ToString();
        if (MultiCharOperators.TryGetValue(op3, out var tokenType))
        {
            Advance(3);
            return new Token(tokenType, null, _currentLine, _currentColumn);
        }

        var op2 = op3.Substring(0, 2);
        if (MultiCharOperators.TryGetValue(op2, out tokenType))
        {
            Advance(2);
            return new Token(tokenType, null, _currentLine, _currentColumn);
        }

        if (SingleCharOperators.TryGetValue(_currentChar, out tokenType))
        {
            Advance();
            return new Token(tokenType, null, _currentLine, _currentColumn);
        }

        throw new Exception($"Unrecognized character: {_currentChar} at line {_currentLine}, column {_currentColumn}");
    }
}