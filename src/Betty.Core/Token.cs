namespace Betty.Core
{
    public enum TokenType
    {
        // Literals
        NumberLiteral, CharLiteral, StringLiteral,

        // Boolean operators
        Not, And, Or,

        // Arithmetic operators
        Plus, Minus, Mul, Div, IntDiv, Caret, Mod,

        // Punctuation
        LParen, RParen, LBrace, RBrace, LBracket, RBracket, 
        Semicolon, Comma, QuestionMark, Colon, DotDot, Arrow,

        // Keywords/identifiers
        Func, Global, TrueKeyword, FalseKeyword, Identifier,
        If, Then, Elif, Else, For, ForEach, In, While, Do, Break, Continue, Return,
        Switch, Case, Default, Underscore,

        // Assignment operators
        Equal, Increment, Decrement, PlusEqual, MinusEqual,
        MulEqual, DivEqual, CaretEqual, ModEqual, IntDivEqual,

        // Comparison operators
        EqualEqual, LessThan, GreaterThan, LessThanOrEqual,
        GreaterThanOrEqual, NotEqual,

        // Special tokens
        EOF,
        Error
    }

    public readonly struct Token(TokenType type, object? value = null, int line = 0, int column = 0)
    {
        public TokenType Type { get; } = type;
        public object? Value { get; } = value;
        public int Line { get; } = line;
        public int Column { get; } = column;
    }
}