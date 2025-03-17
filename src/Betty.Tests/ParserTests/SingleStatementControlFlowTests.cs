namespace Betty.Tests.ParserTests
{
    public class SingleStatementControlFlowTests : ParserTestBase
    {
        [Fact]
        public void ParsesSingleStatementIf()
        {
            var code = "func main() { if (x > 5) x = x - 1; }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            var mainFunction = result!.Functions[0];
            var ifStatement = mainFunction.Body.Statements[0] as IfStatement;

            Assert.NotNull(ifStatement);
            Assert.IsType<ExpressionStatement>(ifStatement.ThenStatement);
            Assert.Null(ifStatement.ElseStatement);
        }

        [Fact]
        public void ParsesSingleStatementElseIf()
        {
            var code = "func main() { if (x > 5) x = x - 1; elif (x < 5) x = x + 1; }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            var mainFunction = result!.Functions[0];
            var ifStatement = mainFunction.Body.Statements[0] as IfStatement;

            Assert.NotNull(ifStatement);
            Assert.Single(ifStatement.ElseIfStatements);
            var elifStatement = ifStatement.ElseIfStatements[0].Statement;
            Assert.IsType<ExpressionStatement>(elifStatement);
        }

        [Fact]
        public void ParsesSingleStatementElse()
        {
            var code = "func main() { if (x > 5) x = x - 1; else x = x + 1; }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            var mainFunction = result!.Functions[0];
            var ifStatement = mainFunction.Body.Statements[0] as IfStatement;

            Assert.NotNull(ifStatement);
            Assert.IsType<ExpressionStatement>(ifStatement.ElseStatement);
        }

        [Fact]
        public void ParsesSingleStatementWhile()
        {
            var code = "func main() { while (x < 10) x = x + 1; }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            var mainFunction = result!.Functions[0];
            var whileStatement = mainFunction.Body.Statements[0] as WhileStatement;

            Assert.NotNull(whileStatement);
            Assert.IsType<ExpressionStatement>(whileStatement.Body);
        }
    }
}