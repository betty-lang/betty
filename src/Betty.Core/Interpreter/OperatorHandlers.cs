using Betty.Core.AST;

namespace Betty.Core.Interpreter
{
    public partial class Interpreter
    {
        private Value HandleBinaryExpression(BinaryOperatorExpression node)
        {
            var left = node.Left.Accept(this);
            var right = node.Right.Accept(this);

            return node.Operator switch
            {
                TokenType.Plus => HandleAddition(left, right),
                TokenType.Minus or TokenType.Mul or TokenType.Div or TokenType.IntDiv or TokenType.Mod or TokenType.Caret => HandleArithmetic(left, right, node.Operator),
                TokenType.And or TokenType.Or => HandleLogical(left, right, node.Operator),
                TokenType.EqualEqual or TokenType.NotEqual or TokenType.LessThan or TokenType.LessThanOrEqual or TokenType.GreaterThan or TokenType.GreaterThanOrEqual => HandleComparison(left, right, node.Operator),
                _ => throw new NotImplementedException($"Operator {node.Operator} not implemented.")
            };
        }

        private static Value HandleAddition(Value left, Value right)
        {
            if (left.Type == ValueType.String || right.Type == ValueType.String)
            {
                return Value.FromString(left.ToString() + right.ToString());
            }

            if (left.Type == ValueType.List)
            {
                var newList = new List<Value>(left.AsList());
                if (right.Type == ValueType.List)
                {
                    newList.AddRange(right.AsList());
                }
                else
                {
                    newList.Add(right);
                }
                return Value.FromList(newList);
            }
            
            if (right.Type == ValueType.List)
            {
                var newList = new List<Value>();
                if (left.Type == ValueType.List)
                {
                    newList.AddRange(left.AsList());
                }
                else
                {
                    newList.Add(left);
                }
                newList.AddRange(right.AsList());
                return Value.FromList(newList);
            }

            return Value.FromNumber(left.AsNumber() + right.AsNumber());
        }

        private static Value HandleArithmetic(Value left, Value right, TokenType op)
        {
            if ((left.Type != ValueType.Number && left.Type != ValueType.Char) ||
                (right.Type != ValueType.Number && right.Type != ValueType.Char))
            {
                throw new InvalidOperationException("Arithmetic operations require numeric operands.");
            }

            var leftNum = left.AsNumber();
            var rightNum = right.AsNumber();

            return op switch
            {
                TokenType.Plus => Value.FromNumber(leftNum + rightNum),
                TokenType.Minus => Value.FromNumber(leftNum - rightNum),
                TokenType.Mul => Value.FromNumber(leftNum * rightNum),
                TokenType.Div => Value.FromNumber(leftNum / rightNum),
                TokenType.IntDiv => Value.FromNumber(Math.Floor(leftNum / rightNum)),
                TokenType.Mod => Value.FromNumber(leftNum % rightNum),
                TokenType.Caret => Value.FromNumber(Math.Pow(leftNum, rightNum)),
                _ => throw new NotImplementedException($"Arithmetic operator {op} not implemented.")
            };
        }

        private static Value HandleLogical(Value left, Value right, TokenType op)
        {
            if (left.Type != ValueType.Boolean || right.Type != ValueType.Boolean)
            {
                throw new InvalidOperationException("Logical operations require boolean operands.");
            }

            var leftBool = left.AsBoolean();
            var rightBool = right.AsBoolean();

            return op switch
            {
                TokenType.And => Value.FromBoolean(leftBool && rightBool),
                TokenType.Or => Value.FromBoolean(leftBool || rightBool),
                _ => throw new NotImplementedException($"Logical operator {op} not implemented.")
            };
        }

        private static Value HandleComparison(Value left, Value right, TokenType op)
        {
            if (left.Type == ValueType.Number || left.Type == ValueType.Char)
            {
                if (right.Type == ValueType.Number || right.Type == ValueType.Char)
                {
                    var leftNum = left.AsNumber();
                    var rightNum = right.AsNumber();
                    return op switch
                    {
                        TokenType.EqualEqual => Value.FromBoolean(leftNum == rightNum),
                        TokenType.NotEqual => Value.FromBoolean(leftNum != rightNum),
                        TokenType.LessThan => Value.FromBoolean(leftNum < rightNum),
                        TokenType.LessThanOrEqual => Value.FromBoolean(leftNum <= rightNum),
                        TokenType.GreaterThan => Value.FromBoolean(leftNum > rightNum),
                        TokenType.GreaterThanOrEqual => Value.FromBoolean(leftNum >= rightNum),
                        _ => throw new InvalidOperationException($"Operator {op} not supported for numeric comparison.")
                    };
                }
            }

            if (left.Type == ValueType.String && right.Type == ValueType.String)
            {
                var leftStr = left.AsString();
                var rightStr = right.AsString();
                return op switch
                {
                    TokenType.EqualEqual => Value.FromBoolean(leftStr == rightStr),
                    TokenType.NotEqual => Value.FromBoolean(leftStr != rightStr),
                    TokenType.LessThan => Value.FromBoolean(string.Compare(leftStr, rightStr, StringComparison.Ordinal) < 0),
                    TokenType.LessThanOrEqual => Value.FromBoolean(string.Compare(leftStr, rightStr, StringComparison.Ordinal) <= 0),
                    TokenType.GreaterThan => Value.FromBoolean(string.Compare(leftStr, rightStr, StringComparison.Ordinal) > 0),
                    TokenType.GreaterThanOrEqual => Value.FromBoolean(string.Compare(leftStr, rightStr, StringComparison.Ordinal) >= 0),
                    _ => throw new InvalidOperationException($"Operator {op} not supported for string comparison.")
                };
            }

            if (left.Type == ValueType.Boolean && right.Type == ValueType.Boolean)
            {
                return op switch
                {
                    TokenType.EqualEqual => Value.FromBoolean(left.AsBoolean() == right.AsBoolean()),
                    TokenType.NotEqual => Value.FromBoolean(left.AsBoolean() != right.AsBoolean()),
                    _ => throw new InvalidOperationException($"Operator {op} not supported for boolean comparison.")
                };
            }

            if (left.Type == ValueType.List && right.Type == ValueType.List)
            {
                return op switch
                {
                    TokenType.EqualEqual => Value.FromBoolean(left.Equals(right)),
                    TokenType.NotEqual => Value.FromBoolean(!left.Equals(right)),
                    _ => throw new InvalidOperationException($"Operator {op} not supported for list comparison.")
                };
            }
            
            if (left.Type == ValueType.Function && right.Type == ValueType.Function)
            {
                return op switch
                {
                    TokenType.EqualEqual => Value.FromBoolean(left.Equals(right)),
                    TokenType.NotEqual => Value.FromBoolean(!left.Equals(right)),
                    _ => throw new InvalidOperationException($"Operator {op} not supported for function comparison.")
                };
            }

            // Default comparison for equality
            if (op == TokenType.EqualEqual) return Value.FromBoolean(left.Equals(right));
            if (op == TokenType.NotEqual) return Value.FromBoolean(!left.Equals(right));

            throw new InvalidOperationException($"Cannot compare {left.Type} and {right.Type}.");
        }
    }
}
