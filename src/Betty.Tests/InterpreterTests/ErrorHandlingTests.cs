using Betty.Core;
using Betty.Tests.TestUtilities;

namespace Betty.Tests.InterpreterTests
{
    public class ErrorHandlingTests : InterpreterTestBase
    {
        [Fact]
        public void Interpreter_Catches_Multiple_Errors()
        {
            var code = @"
func main() {
    a = 1;
    b = .; // Invalid character
    c = 'unterminated;
    d = ""another unterminated;
    e = 1 +;
}";

            var lexer = new Lexer(code);
            var tokens = lexer.GetTokens();
            var parser = new Parser(tokens);
            parser.Parse();

            var errorMessages = lexer.Errors.Concat(parser.Errors).Select(e => e.Message).ToList();

            Assert.Contains("Unrecognized character: .", errorMessages);
            Assert.Contains("Unterminated character literal.", errorMessages);
            Assert.Contains("Unterminated string literal.", errorMessages);
            Assert.Contains("Unexpected token: Error", errorMessages);
            Assert.Contains("Unexpected token: Expected Semicolon, found Identifier", errorMessages);
        }
    }
}
