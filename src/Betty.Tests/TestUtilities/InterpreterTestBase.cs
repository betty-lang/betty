using Betty.Core.Interpreter;

namespace Betty.Tests.TestUtilities
{
    public class InterpreterTestBase
    {
        protected static Interpreter SetupInterpreter(string code, bool customSetup = false)
        {
            return new Interpreter(
                new Parser(
                    new Lexer(
                        customSetup ? code : $"func main() {{ {code} }}")));
        }
    }
}