using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public enum ValueType
    {
        Number,
        String,
        Boolean,
        Char,
        List,
        Function,
        None
    }

    public sealed class Value : IEquatable<Value>
    {
        public readonly ValueType Type;
        private readonly double _number;
        private readonly char _char;
        private readonly int _stringId;
        private readonly bool _boolean;
        private readonly List<Value>? _list;
        private readonly FunctionExpression? _function;

        private Value(ValueType type)
        {
            Type = type;
        }

        private Value(double number) : this(ValueType.Number)
        {
            _number = number;
        }

        private Value(int stringId) : this(ValueType.String)
        {
            _stringId = stringId;
        }

        private Value(bool boolean) : this(ValueType.Boolean)
        {
            _boolean = boolean;
        }

        private Value(char character) : this(ValueType.Char)
        {
            _char = character;
        }

        private Value(List<Value> list) : this(ValueType.List)
        {
            _list = list;
        }

        private Value(FunctionExpression function) : this(ValueType.Function)
        {
            _function = function;
        }

        public static Value FromString(string str)
        {
            int stringId = StringTable.AddString(str);
            return new Value(stringId);
        }

        public static Value FromNumber(double number) => new(number);
        public static Value FromBoolean(bool boolean) => new(boolean);
        public static Value FromChar(char character) => new(character);
        public static Value FromList(List<Value> list) => new(list);
        public static Value FromFunction(FunctionExpression function) => new(function);
        public static Value None() => new(ValueType.None);

        public char AsChar()
        {
            if (Type != ValueType.Char)
                throw new InvalidOperationException($"Expected a character, but got {Type}.");
            return _char;
        }

        public double AsNumber()
        {
            if (Type == ValueType.Char)
                return _char;

            if (Type != ValueType.Number)
                throw new InvalidOperationException($"Expected a {ValueType.Number}, but got {Type}.");
            return _number;
        }

        public string AsString()
        {
            if (Type != ValueType.String)
                throw new InvalidOperationException($"Expected a {ValueType.String}, but got {Type}.");
            return StringTable.GetString(_stringId);
        }

        public bool AsBoolean()
        {
            if (Type != ValueType.Boolean)
                throw new InvalidOperationException($"Expected a {ValueType.Boolean}, but got {Type}.");
            return _boolean;
        }

        public List<Value> AsList()
        {
            return Type switch
            {
                ValueType.List => _list ?? new List<Value>(),
                ValueType.String => StringTable.GetString(_stringId)
                    .Select(c => FromChar(c))
                    .ToList(),
                _ => throw new InvalidOperationException($"Expected a {ValueType.List} or {ValueType.String}, but got {Type}.")
            };
        }

        public FunctionExpression AsFunction()
        {
            if (Type != ValueType.Function)
                throw new InvalidOperationException($"Expected a {ValueType.Function}, but got {Type}.");
            return _function!;
        }

        public Value Clone()
        {
            return Type switch
            {
                ValueType.List => FromList(_list!.Select(v => v.Clone()).ToList()),
                _ => this 
            };
        }

        public override string ToString()
        {
            return Type switch
            {
                ValueType.Number => _number.ToString(),
                ValueType.String => StringTable.GetString(_stringId),
                ValueType.Boolean => _boolean.ToString(),
                ValueType.Char => _char.ToString(),
                ValueType.List => $"[{string.Join(", ", _list!.Select(item => item.ToString()))}]",
                ValueType.Function => "<function>",
                ValueType.None => "none",
                _ => throw new InvalidOperationException($"Unknown type {Type}.")
            };
        }

        public bool Equals(Value? other)
        {
            if (other is null) return false;
            if (Type != other.Type) return false;

            return Type switch
            {
                ValueType.Number => _number == other._number,
                ValueType.String => _stringId == other._stringId,
                ValueType.Boolean => _boolean == other._boolean,
                ValueType.Char => _char == other._char,
                ValueType.List => _list!.SequenceEqual(other._list!),
                ValueType.Function => _function == other._function,
                ValueType.None => true,
                _ => false
            };
        }

        public override bool Equals(object? obj) => Equals(obj as Value);

        public override int GetHashCode()
        {
            return Type switch
            {
                ValueType.Number => HashCode.Combine(Type, _number),
                ValueType.String => HashCode.Combine(Type, _stringId),
                ValueType.Boolean => HashCode.Combine(Type, _boolean),
                ValueType.Char => HashCode.Combine(Type, _char),
                ValueType.List => HashCode.Combine(Type, _list),
                ValueType.Function => HashCode.Combine(Type, _function),
                ValueType.None => Type.GetHashCode(),
                _ => 0
            };
        }
    }
}