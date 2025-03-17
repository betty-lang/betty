namespace Betty.Tests.InterpreterTests
{
    public class ExpressionStatementTests : InterpreterTestBase
    {
        [Fact]
        public void ExpressionStatement_ReturnsNone()
        {
            var code = "x = 5; x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(Core.Interpreter.ValueType.None, result.Type);
        }

        [Fact]
        public void ExpressionStatement_PrefixDecrementOperator_WithUnaryMinus_ModifiesVariable()
        {
            var code = "x = 5; - --x + 5; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }
    }
}