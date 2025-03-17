namespace Betty.Tests.InterpreterTests
{
    public class IntrinsicFunctionTests : InterpreterTestBase
    {
        [Fact]
        public void CeilFunction_ReturnsCorrectValue()
        {
            var code = "return ceil(5.5);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            Assert.Equal(6.0, result.AsNumber());
        }

        [Fact]
        public void LengthFunction_ReturnsCorrectValue()
        {
            var code = "return len(\"Hello\");";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void PowFunction_ReturnsCorrectValue()
        {
            var code = "return pow(2, 3);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            Assert.Equal(8.0, result.AsNumber());
        }

        [Fact]
        public void ConvertToStringFunction_ReturnsCorrectValue()
        {
            var code = "return tostr(5);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            Assert.Equal("5", result.AsString());
        }

        [Fact]
        public void ConvertToNumberFunction_ReturnsCorrectValue()
        {
            var code = "return tonum(\"5\");";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void ConvertToBooleanFunction_ReturnsCorrectValue()
        {
            var code = "return tobool(5);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            Assert.True(result.AsBoolean());
        }

        [Fact]
        public void PrintFunction_PrintsCorrectValue()
        {
            var code = "print(tostr(2 + 3));";
            var interpreter = SetupInterpreter(code);
            var output = new StringWriter();
            Console.SetOut(output);

            interpreter.Interpret();

            Assert.Equal("5", output.ToString());
        }

        [Fact]
        public void PrintFunction_PrintsCorrectValueWithNewLine()
        {
            var code = """print("Hello World\n");""";
            var interpreter = SetupInterpreter(code);
            var output = new StringWriter();
            Console.SetOut(output);

            interpreter.Interpret();

            Assert.Equal("Hello World\n", output.ToString());
        }

        [Fact]
        public void PrintFunction_PrintsCorrectValue_WithStringConcatenation()
        {
            var code = """print("Hello" + " " + "World"); """;
            var interpreter = SetupInterpreter(code);
            var output = new StringWriter();
            Console.SetOut(output);

            interpreter.Interpret();

            Assert.Equal("Hello World", output.ToString());
        }

        [Fact]
        public void PrintFunction_PrintsCorrectValueWithMultipleArguments()
        {
            var code = "print(2, 3, 4);";
            var interpreter = SetupInterpreter(code);
            var output = new StringWriter();
            Console.SetOut(output);

            interpreter.Interpret();

            Assert.Equal("234", output.ToString());
        }

        [Fact]
        public void PrintLineFunction_PrintsCorrectValueWithMultipleArguments()
        {
            var code = "println(2, 3, 4);";
            var interpreter = SetupInterpreter(code);
            var output = new StringWriter();
            Console.SetOut(output);

            interpreter.Interpret();

            Assert.Equal("234\n", output.ToString());
        }

        [Fact]
        public void InputFunction_ReturnsCorrectValue()
        {
            var code = "return tonum(input());";
            var interpreter = SetupInterpreter(code);
            var input = new StringReader("5");
            Console.SetIn(input);

            var result = interpreter.Interpret();

            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void InputFunction_ReturnsCorrectValueWithPrompt()
        {
            var code = "return tonum(input(\"Enter a number: \"));";
            var interpreter = SetupInterpreter(code);
            var input = new StringReader("5");
            Console.SetIn(input);

            var result = interpreter.Interpret();

            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void StringConcatFunction_ReturnsCorrectValue()
        {
            var code = "return concat(\"Hello\", \" \", \"World\");";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            Assert.Equal("Hello World", result.AsString());
        }

        [Fact]
        public void StringConcatFunction_ReturnsCorrectValueWithNumber()
        {
            var code = "return concat(\"Hello\", 5);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            Assert.Equal("Hello5", result.AsString());
        }
    }
}