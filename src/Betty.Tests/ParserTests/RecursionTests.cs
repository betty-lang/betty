namespace Betty.Tests.ParserTests
{
    public class RecursionTests : ParserTestBase
    {
        [Fact]
        public void Parse_RecursiveFunction_DefinesCorrectly()
        {
            // Example of a simple recursive function
            var code = @"
                func factorial(n) {
                    if (n <= 1) return 1;
                    return n * factorial(n - 1);
                }
            ";

            var lexer = new Lexer(code);
            var tokens = lexer.GetTokens();
            var parser = new Parser(tokens);
            var programNode = parser.Parse() as Program;

            // Assert that the program node is not null
            Assert.NotNull(programNode);

            // Assert that the function "factorial" is defined
            var functionNode = programNode.Functions.FirstOrDefault(f => f.FunctionName == "factorial");
            Assert.NotNull(functionNode);

            // Assert that the function has one parameter named "n"
            Assert.Single(functionNode.Parameters);
            Assert.Equal("n", functionNode.Parameters[0]);

            // Assert that the function body contains an if statement and a return statement
            var compoundStatement = functionNode.Body;
            Assert.NotNull(compoundStatement);
            Assert.Equal(2, compoundStatement.Statements.Count);

            // Assert the structure of the if statement
            var ifStatement = compoundStatement.Statements[0] as IfStatement;
            Assert.NotNull(ifStatement);
            // Additional assertions to check the condition and the branches of the if statement

            var returnStatement = compoundStatement.Statements[1] as ReturnStatement;
            Assert.NotNull(returnStatement);

            // Check for the recursive call in the return statement
            var binaryOperatorNode = returnStatement.ReturnValue as BinaryOperatorExpression;
            Assert.NotNull(binaryOperatorNode);

            var functionCallNode = binaryOperatorNode.Right as FunctionCall; // Assuming the recursive call is on the right-hand side
            Assert.NotNull(functionCallNode);
            Assert.Equal("factorial", functionCallNode.FunctionName);

            // Check that the argument of the recursive call is "n - 1"
            Assert.Single(functionCallNode.Arguments);
            var argumentExpression = functionCallNode.Arguments[0] as BinaryOperatorExpression;
            Assert.NotNull(argumentExpression);
            Assert.Equal(TokenType.Minus, argumentExpression.Operator);
        }
    }
}