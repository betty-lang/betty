namespace Betty.Tests.InterpreterTests
{
    public class UnaryOperatorTests : InterpreterTestBase
    {
        [Fact]
        public void PrefixDecrementOperator_CanBeUsedInAssignment()
        {
            var code = "x = 5; --x; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }

        [Fact]
        public void PrefixAndPostfixIncrementOperators_ReturnCorrectValue()
        {
            var code = "x = 0; return ++x + x++;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(2.0, result.AsNumber());
        }

        [Fact]
        public void PrefixAndPostfixDecrementOperators_ReturnCorrectValue()
        {
            var code = "x = 0; return --x + x--;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(-2.0, result.AsNumber());
        }

        [Fact]
        public void PostfixIncrementOperator_ReturnsCorrectValue()
        {
            var code = "x = 5; return x++;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void PostfixIncrementOperator_ModifiesVariable()
        {
            var code = "x = 5; x++; return x + 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(9.0, result.AsNumber());
        }

        [Fact]
        public void PostfixDecrementOperator_ReturnsCorrectValue()
        {
            var code = "x = 5; return x--;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void PostfixDecrementOperator_ModifiesVariable()
        {
            var code = "x = 5; x--; return x + 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(7.0, result.AsNumber());
        }

        [Fact]
        public void PrefixIncrementOperator_ReturnsCorrectValue()
        {
            var code = "x = 5; return ++x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(6.0, result.AsNumber());
        }

        [Fact]
        public void PrefixIncrementOperator_ModifiesVariable()
        {
            var code = "x = 5; return ++x + 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(9.0, result.AsNumber());
        }

        [Fact]
        public void PrefixDecrementOperator_ReturnsCorrectValue()
        {
            var code = "x = 5; return --x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }

        [Fact]
        public void PrefixDecrementOperator_ModifiesVariable()
        {
            var code = "x = 5; return --x + 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(7.0, result.AsNumber());
        }

        [Fact]
        public void UnaryPlusOperator_DoesNotChangeSign()
        {
            var code = "return +5;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void UnaryMinusOperator_NegatesValue()
        {
            var code = "return -5;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(-5.0, result.AsNumber());
        }

        [Fact]
        public void NestedUnaryOperators_EvaluatesCorrectly()
        {
            var code = "return -(-5);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void UnaryMinusOperator_HasHigherPrecedenceThanPowerOperator()
        {
            var code = "return -2 ^ 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(-8.0, result.AsNumber());
        }

        [Fact]
        public void UnaryMinusOperator_HasLowerPrecedenceThanMultiplicationOperator()
        {
            var code = "return -2 * 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(-6.0, result.AsNumber());
        }

        [Fact]
        public void UnaryMinusOperator_HasLowerPrecedenceThanDivisionOperator()
        {
            var code = "return -6 / 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(-2.0, result.AsNumber());
        }

        [Fact]
        public void UnaryMinusOperator_HasLowerPrecedenceThanAdditionOperator()
        {
            var code = "return -2 + 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsNumber());
        }

        [Fact]
        public void UnaryMinusOperator_HasLowerPrecedenceThanSubtractionOperator()
        {
            var code = "return -2 - 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(-5.0, result.AsNumber());
        }

        [Fact]
        public void UnaryMinusOperator_HasHigherPrecedenceThanParentheses()
        {
            var code = "return -(2 + 3);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(-5.0, result.AsNumber());
        }

    }
}