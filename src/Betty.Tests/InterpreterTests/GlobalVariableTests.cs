namespace Betty.Tests.InterpreterTests
{
    public class GlobalVariableTests : InterpreterTestBase
    {
        [Fact]
        public void GlobalVariable_CanBeShadowedByFunctionParameter()
        {
            var code = """
                global x;

                func foo(x) {
                    x = 4;
                }

                func main() {
                    x = 5;
                    foo(3);
                    return x;
                }
                """;
            var interpreter = SetupInterpreter(code, true);

            var result = interpreter.Interpret();

            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void GlobalVariable_CanBeAssignedAndRead()
        {
            var code = """
                global x;

                func main() {
                    x = 5;
                    return x;
                }
                """;
            var interpreter = SetupInterpreter(code, true);

            var result = interpreter.Interpret();

            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void GlobalVariable_CanBeAssignedAndReadInMultipleFunctions()
        {
            var code = """
                global x;

                func main() {
                    x = 5;
                    other();
                    return x;
                }

                func other() {
                    x = 3;
                }
                """;
            var interpreter = SetupInterpreter(code, true);

            var result = interpreter.Interpret();

            Assert.Equal(3.0, result.AsNumber());
        }
    }
}