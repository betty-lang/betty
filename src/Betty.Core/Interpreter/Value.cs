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

    public class ValueList
    {
        public List<Value> Items { get; } = new List<Value>();

        public ValueList() { }

        public ValueList(IEnumerable<Value> initial)
        {
            Items.AddRange(initial);
        }

        public ValueList Clone()
        {
            // Deep copy: create new list with cloned elements
            var clonedItems = new List<Value>();
            foreach (var item in Items)
            {
                clonedItems.Add(Value.DeepCopy(item));
            }
            return new ValueList(clonedItems);
        }
    }

    public readonly struct Value : IEquatable<Value>
    {
        public readonly ValueType Type;
        private readonly double _number;
        private readonly char _char;
        private readonly int _stringId;
        private readonly bool _boolean;
        private readonly ValueList? _list;
        private readonly FunctionExpression? _function;

        private Value(ValueType type) : this()
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

        private Value(ValueList list) : this(ValueType.List)
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

        public static Value FromNumber(double number) => new Value(number);
        public static Value FromBoolean(bool boolean) => new Value(boolean);
        public static Value FromChar(char character) => new Value(character);
        public static Value FromList(List<Value> list) => new Value(new ValueList(list));
        public static Value FromFunction(FunctionExpression function) => new Value(function);
        public static Value None() => new Value(ValueType.None);

        public readonly char AsChar()
        {
            if (Type != ValueType.Char)
                throw new InvalidOperationException($"Expected a character, but got {Type}.");
            return _char;
        }

        public readonly double AsNumber()
        {
            if (Type == ValueType.Char)
                return _char + 0; // Convert the character to its numeric value.

            if (Type != ValueType.Number)
                throw new InvalidOperationException($"Expected a {ValueType.Number}, but got {Type}.");
            return _number;
        }

        public readonly string AsString()
        {
            if (Type != ValueType.String)
                throw new InvalidOperationException($"Expected a {ValueType.String}, but got {Type}.");
            return StringTable.GetString(_stringId);
        }

        public readonly bool AsBoolean()
        {
            if (Type != ValueType.Boolean)
                throw new InvalidOperationException($"Expected a {ValueType.Boolean}, but got {Type}.");
            return _boolean;
        }

        public List<Value> AsList()
        {
            return Type switch
            {
                ValueType.List => _list?.Items ?? new List<Value>(),
                ValueType.String => StringTable.GetString(_stringId)
                    .Select(c => Value.FromChar(c))
                    .ToList(),
                _ => throw new InvalidOperationException($"Expected a {ValueType.List} or {ValueType.String}, but got {Type}.")
            };
        }

        public readonly FunctionExpression AsFunction()
        {
            if (Type != ValueType.Function)
                throw new InvalidOperationException($"Expected a {ValueType.Function}, but got {Type}.");
            return _function!;
        }

        public static Value DeepCopy(Value value)
        {
            return value.Type switch
            {
                // Primitive types are immutable, so they can be returned as-is
                ValueType.Number => Value.FromNumber(value.AsNumber()),
                ValueType.String => Value.FromString(value.AsString()),
                ValueType.Boolean => Value.FromBoolean(value.AsBoolean()),
                ValueType.Char => Value.FromChar(value.AsChar()),

                // For lists, recursively deep copy each element
                ValueType.List => Value.FromList(DeepCopyList(value.AsList())),

                // Functions can be returned as-is
                ValueType.Function => Value.FromFunction(value.AsFunction()),

                // None type can be returned as-is
                ValueType.None => Value.None(),

                _ => throw new InvalidOperationException($"Unsupported type for deep copy: {value.Type}")
            };
        }

        private static List<Value> DeepCopyList(List<Value> originalList)
        {
            // Create a new list with deep copied elements
            var deepCopiedList = new List<Value>();
            foreach (var element in originalList)
            {
                // Recursively deep copy each element
                deepCopiedList.Add(DeepCopy(element));
            }

            return deepCopiedList;
        }

        public override string ToString()
        {
            return Type switch
            {
                ValueType.Number => _number.ToString(),
                ValueType.String => StringTable.GetString(_stringId),
                ValueType.Boolean => _boolean.ToString(),
                ValueType.Char => _char.ToString(),
                ValueType.List => _list == null
                    ? "[]"
                    : $"[{string.Join(", ", _list.Items.Select(item => item.ToString()))}]",
                ValueType.Function => "<function>", 
                _ => throw new InvalidOperationException($"Unknown type {Type}.")
            };
        }

        public bool Equals(Value other)
        {
            if (Type != other.Type)
                return false;

            return Type switch
            {
                ValueType.Number => _number == other._number,
                ValueType.String => _stringId == other._stringId,
                ValueType.Boolean => _boolean == other._boolean,
                ValueType.Char => _char == other._char,
                ValueType.List => _list == null && other._list == null
                    || (_list != null && other._list != null &&
                        _list.Items.SequenceEqual(other._list.Items)),
                ValueType.Function => _function == other._function,
                ValueType.None => true,
                _ => throw new InvalidOperationException($"Unknown type {Type}.")
            };
        }

        public override bool Equals(object? obj)
        {
            return obj is Value other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
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
                    _ => throw new InvalidOperationException($"Unknown type {Type}.")
                };
            }
        }

        public static bool operator ==(Value left, Value right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Value left, Value right)
        {
            return !(left == right);
        }
    }
}