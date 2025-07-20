namespace Betty.Tests.ParserTests
{
    public class FunctionTests : ParserTestBase
    {
        [Fact]
        public void ParsesValidFunctionDefinition_CreatesCorrectAST()
        {
            var code = "func add(a, b) { return a + b; } func main() { return add(1, 2); }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            Assert.Equal(2, result.Functions.Count);

            var addFunctionNode = result.Functions[0];
            Assert.Equal("add", addFunctionNode.FunctionName);
            Assert.Equal(2, addFunctionNode.Parameters.Count);
            Assert.Equal("a", addFunctionNode.Parameters[0]);
            Assert.Equal("b", addFunctionNode.Parameters[1]);
            Assert.IsType<ReturnStatement>(addFunctionNode.Body.Statements[0]);

            var mainFunctionNode = result.Functions[1];
            Assert.Equal("main", mainFunctionNode.FunctionName);
            Assert.Empty(mainFunctionNode.Parameters);
        }

        [Fact]
        public void ParsesSimpleFunctionDeclaration()
        {
            var code = "func main() {}";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            Assert.Single(result.Functions);
            var function = result.Functions[0];
            Assert.Equal("main", function.FunctionName);
            Assert.Empty(function.Parameters);
            Assert.Empty(function.Body.Statements);
        }

        [Fact]
        public void ParsesFunctionWithParameters()
        {
            var code = "func add(a, b) { return a + b; }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            var function = result.Functions[0];
            Assert.Equal("add", function.FunctionName);
            Assert.Equal(2, function.Parameters.Count);
            Assert.Contains("a", function.Parameters);
            Assert.Contains("b", function.Parameters);
        }

        [Fact]
        public void HandlesFunctionWithNoBody()
        {
            var code = "func emptyFunc()";
            var parser = SetupParser(code);
            parser.Parse();

            Assert.NotEmpty(parser.Errors);
        }

        [Fact]
        public void ParsesFunctionWithComplexBody()
        {
            var code = "func complex() { x = 5; if (x > 3) { x = x - 1; } return x; }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            var function = result.Functions[0];
            Assert.Equal("complex", function.FunctionName);
            Assert.Equal(3, function.Body.Statements.Count);
        }
    }
}