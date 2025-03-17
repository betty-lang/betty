namespace Betty.Tests.InterpreterTests
{
    public class ArithmeticExpressionTests : InterpreterTestBase
    {
        [Fact]
        public void PostfixAndPrefixAddition_WithAssignment_ReturnsCorrectValue()
        {
            var code = "x = 0; x = x++ + ++x; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(2, result.AsNumber());
        }

        [Fact]
        public void CharMultiplication_ReturnsCorrectProduct()
        {
            var code = "return 'a' * 'b';";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal('a' * 'b', result.AsNumber());
        }

        [Fact]
        public void CharSubtraction_ReturnsCorrectDifference()
        {
            var code = "return 'a' - 'b';";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal('a' - 'b', result.AsNumber());
        }

        [Fact]
        public void CharIntegerDivision_ReturnsCorrectQuotient()
        {
            var code = "return 'a' // 'b';";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal('a' / 'b', result.AsNumber());
        }

        [Fact]
        public void CharAddition_ReturnsCorrectSum()
        {
            var code = "return 'a' + 'b';";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal('a' + 'b', result.AsNumber());
        }

        [Fact]
        public void Addition_ReturnsCorrectSum()
        {
            var code = "return 2 + 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void Subtraction_ReturnsCorrectDifference()
        {
            var code = "return 5 - 2;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(3.0, result.AsNumber());
        }

        [Fact]
        public void Multiplication_ReturnsCorrectProduct()
        {
            var code = "return 3 * 4;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(12.0, result.AsNumber());
        }

        [Fact]
        public void Division_ReturnsCorrectQuotient()
        {
            var code = "return 10 / 2;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void MixedOperations_ReturnsCorrectValue()
        {
            var code = "return 2 + 3 * 4 - 5;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(9.0, result.AsNumber());
        }

        [Fact]
        public void PowerOperation_ReturnsCorrectValue()
        {
            var code = "return 2 ^ 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(8.0, result.AsNumber());
        }

        [Fact]
        public void CombinedAdditionAndPower_ReturnsCorrectValue()
        {
            var code = "return 2 + 3 ^ 2;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(11.0, result.AsNumber());
        }

        [Fact]
        public void ComplexExpressionWithParentheses_ReturnsCorrectValue()
        {
            var code = "return (2 + 3) * (4 - 1);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(15.0, result.AsNumber());
        }

        [Fact]
        public void NestedPowers_ReturnsCorrectValue()
        {
            var code = "return 2 ^ (1 + 1 ^ 3);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }

        [Fact]
        public void DivisionByZero_IsPositiveInfinity()
        {
            var code = "return 10 / 0;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(double.PositiveInfinity, result.AsNumber());
        }

        [Fact]
        public void DeeplyNestedExpressions_EvaluatesCorrectly()
        {
            var code = "return (((((1 + 2) * 3) - 4) / 5) ^ 2);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(Math.Pow(((1 + 2) * 3 - 4) / 5.0, 2), result.AsNumber());
        }

        [Fact]
        public void NegativeExponentiation_ReturnsCorrectValue()
        {
            var code = "return 2 ^ -3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0 / 8, result.AsNumber());
        }

        [Fact]
        public void ModuloOperator_ReturnsCorrectValue()
        {
            var code = "return 5 % 2;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsNumber());
        }

        [Fact]
        public void ModuloOperator_HasHigherPrecedenceThanAdditionOperator()
        {
            var code = "return 5 + 2 % 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(7.0, result.AsNumber());
        }

        [Fact]
        public void ModuloOperator_HasHigherPrecedenceThanSubtractionOperator()
        {
            var code = "return 5 - 2 % 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(3.0, result.AsNumber());
        }
    }
}