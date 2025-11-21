using Betty.Core;
using Betty.Core.AST;
using Betty.Tests.TestUtilities;

namespace Betty.Tests.ParserTests
{
    public class ErrorRecoveryTests : ParserTestBase
    {
        [Fact]
        public void ReportsMultipleErrorsInExpressions()
        {
            // Multiple syntax errors in a single function
            var code = @"
                func main() {
                    x = 5 + ;
                    y = * 10;
                    z = 15;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse();

            // Should have at least 2 errors
            Assert.True(parser.Errors.Count >= 2);
            Assert.Contains("Unexpected token", parser.Errors[0]);
        }

        [Fact]
        public void ReportsMultipleErrorsInStatements()
        {
            // Missing semicolons and braces
            var code = @"
                func main() {
                    x = 5
                    y = 10
                    z = 15;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse();

            // Should report missing semicolons
            Assert.True(parser.Errors.Count >= 2);
        }

        [Fact]
        public void ContinuesParsingAfterExpressionError()
        {
            // Error in first statement, but continues to parse second
            var code = @"
                func main() {
                    x = 5 + ;
                    y = 10;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            Assert.Single(result.Functions);
            var mainFunction = result.Functions[0];
            Assert.Equal("main", mainFunction.FunctionName);

            // Should have parsed the function despite the error
            Assert.NotEmpty(mainFunction.Body.Statements);
            Assert.NotEmpty(parser.Errors);
        }

        [Fact]
        public void ContinuesParsingAfterStatementError()
        {
            // Error in if statement, but continues to parse next statement
            var code = @"
                func main() {
                    if (x > 5 { y = 10; }
                    z = 15;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            Assert.Single(result.Functions);

            // Should have parsed despite the error
            Assert.NotEmpty(parser.Errors);
        }

        [Fact]
        public void ReportsUnterminatedStringLiteral()
        {
            var code = @"
                func main() {
                    x = ""unterminated string;
                    y = 10;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse();

            // Lexer should report unterminated string
            Assert.NotEmpty(parser.Errors);
            Assert.Contains("Unterminated string", parser.Errors[0]);
        }

        [Fact]
        public void ReportsInvalidEscapeSequence()
        {
            var code = @"
                func main() {
                    x = ""invalid\qescape"";
                    y = 10;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse();

            // Lexer should report invalid escape sequence
            Assert.NotEmpty(parser.Errors);
            Assert.Contains("escape sequence", parser.Errors[0]);
        }

        [Fact]
        public void ReportsUnrecognizedCharacter()
        {
            var code = @"
                func main() {
                    x = 5 @ 10;
                    y = 15;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse();

            // Lexer should report unrecognized character
            Assert.NotEmpty(parser.Errors);
            // The error might be reported in any of the error messages
            Assert.Contains(parser.Errors, e => e.Contains("Unrecognized character") || e.Contains("@"));
        }

        [Fact]
        public void ReportsMultipleErrorsAcrossFunctions()
        {
            var code = @"
                func first() {
                    x = 5 + ;
                }

                func second() {
                    y = * 10;
                }

                func third() {
                    z = 15;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            // Should parse all three functions despite errors
            Assert.Equal(3, result.Functions.Count);

            // Should have errors from both first and second functions
            Assert.True(parser.Errors.Count >= 2);
        }

        [Fact]
        public void RecoversfromMissingClosingBrace()
        {
            var code = @"
                func first() {
                    x = 5;

                func second() {
                    y = 10;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            // Should try to parse both functions
            Assert.NotEmpty(parser.Errors);
        }

        [Fact]
        public void ReportsErrorInNestedStatements()
        {
            var code = @"
                func main() {
                    if (x > 5) {
                        y = 10 +;
                        z = 15;
                    }
                    a = 20;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            Assert.Single(result.Functions);

            // Should report error but continue parsing
            Assert.NotEmpty(parser.Errors);
        }

        [Fact]
        public void HandlesEmptyCharLiteral()
        {
            var code = @"
                func main() {
                    x = '';
                    y = 'a';
                }";
            var parser = SetupParser(code);
            var result = parser.Parse();

            // Should report empty char literal error
            Assert.NotEmpty(parser.Errors);
            Assert.Contains("Empty character literal", parser.Errors[0]);
        }

        [Fact]
        public void HandlesMultipleDotsInNumber()
        {
            var code = @"
                func main() {
                    x = 3.14.15;
                    y = 10;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse();

            // Should report invalid number format
            Assert.NotEmpty(parser.Errors);
            Assert.Contains("multiple dots", parser.Errors[0]);
        }

        [Fact]
        public void ContinuesAfterMissingSemicolonInForLoop()
        {
            var code = @"
                func main() {
                    for (i = 0; i < 10; i = i + 1) {
                        x = i
                        y = i * 2;
                    }
                }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            // Should report missing semicolon but continue
            Assert.NotEmpty(parser.Errors);
        }

        [Fact]
        public void NoErrorsForValidCode()
        {
            var code = @"
                func main() {
                    x = 5;
                    y = 10;
                    z = x + y;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            Assert.Empty(parser.Errors); // No errors for valid code
        }

        [Fact]
        public void HandlesErrorInFunctionParameters()
        {
            var code = @"
                func add(a, , c) {
                    return a + c;
                }

                func main() {
                    x = 5;
                }";
            var parser = SetupParser(code);
            var result = parser.Parse() as Program;

            Assert.NotNull(result);
            // Should report error in parameter list
            Assert.NotEmpty(parser.Errors);
        }
    }
}
