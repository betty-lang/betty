namespace Betty.Tests.ParserTests
{
    public class ControlFlowTests : ParserTestBase
    {
        [Fact]
        public void ParsesForStatement()
        {
            var code = "func main() { for (x = 0; x < 10; x = x + 1) { y = y + 1; } }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            var mainFunction = result.Functions[0];
            var forStatement = mainFunction.Body.Statements[0] as ForStatement;

            Assert.NotNull(forStatement);
            Assert.NotNull(forStatement.Initializer);
            Assert.NotNull(forStatement.Condition);
            Assert.NotNull(forStatement.Increment);
            Assert.Single((forStatement.Body as CompoundStatement)!.Statements);
        }

        [Fact]
        public void ParsesIfStatement()
        {
            var code = "func main() { if (x > 5) { x = x - 1; } }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            var mainFunction = result.Functions[0];
            var ifStatement = mainFunction.Body.Statements[0] as IfStatement;

            Assert.NotNull(ifStatement);
            Assert.NotNull(ifStatement.Condition);
            Assert.NotNull(ifStatement.ThenStatement);
            Assert.Null(ifStatement.ElseStatement);
        }

        [Fact]
        public void ParsesWhileStatement()
        {
            var code = "func main() { while (x < 10) { x = x + 1; } }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            var mainFunction = result.Functions[0];
            var whileStatement = mainFunction.Body.Statements[0] as WhileStatement;

            Assert.NotNull(whileStatement);
            Assert.NotNull(whileStatement.Condition);
            Assert.Single((whileStatement.Body as CompoundStatement)!.Statements);
        }

        [Fact]
        public void ParsesCompoundStatement()
        {
            var code = "func main() { { x = 0; y = 1; } }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            var mainFunction = result.Functions[0];
            var compoundStatement = mainFunction.Body.Statements[0] as CompoundStatement;

            Assert.NotNull(compoundStatement);
            Assert.Equal(2, compoundStatement.Statements.Count);
        }

        [Fact]
        public void ParsesNestedControlFlowStatements()
        {
            var code = "func main() { if (x > 5) { while (y < 10) { y = y + 1; } } }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            var mainFunction = result.Functions[0];
            var ifStatement = mainFunction.Body.Statements[0] as IfStatement;
            var whileStatement = (ifStatement!.ThenStatement as CompoundStatement)!.Statements[0] as WhileStatement;

            Assert.NotNull(ifStatement);
            Assert.NotNull(whileStatement);
            Assert.Single((whileStatement.Body as CompoundStatement)!.Statements);
        }

        [Fact]
        public void ParsesIfElseStatement()
        {
            var code = "func main() { if (x > 5) { x = x - 1; } else { x = x + 1; } }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            var mainFunction = result.Functions[0];
            var ifStatement = mainFunction.Body.Statements[0] as IfStatement;

            Assert.NotNull(ifStatement);
            Assert.NotNull(ifStatement.ElseStatement);
            Assert.Single((ifStatement.ThenStatement as CompoundStatement)!.Statements);
            Assert.Single((ifStatement.ElseStatement as CompoundStatement)!.Statements);
        }
    }
}