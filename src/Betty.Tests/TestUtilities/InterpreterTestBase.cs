using Betty.Core.Interpreter;

namespace Betty.Tests.TestUtilities
{
    public class InterpreterTestBase
    {
        protected static Interpreter SetupInterpreter(string code, bool customSetup = false)
        {
            var lexer = new Lexer(customSetup ? code : $"func main() {{ {code} }}");
            var tokens = lexer.GetTokens();
            var parser = new Parser(tokens);
            var program = (Program)parser.Parse();
            return new Interpreter(program);
        }
    }
}