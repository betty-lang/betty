namespace Betty.Tests.InterpreterTests
{
    public class ForEachStatementTests : InterpreterTestBase
    {
        [Fact]
        public void ForEachStatement_WithList()
        {
            var code = @"
                list = [1, 2, 3, 4, 5];
                sum = 0;
                foreach (i in list) {
                    sum += i;
                }
                return sum;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            Assert.Equal(15, result.AsNumber());
        }

        [Fact]
        public void ForEachStatement_WithString()
        {
            var code = """
                str = "hello";
                result = "";
                foreach (c in str) {
                    result += c;
                }
                return result;
            """;
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            Assert.Equal("hello", result.AsString());
        }

        [Fact]
        public void ForEachStatement_WithNestedList()
        {
            var code = @"
                list = [[1, 2], [3, 4], [5, 6]];
                sum = 0;
                foreach (innerList in list) {
                    foreach (i in innerList) {
                        sum += i;
                    }
                }
                return sum;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            Assert.Equal(21, result.AsNumber());
        }
    }
}