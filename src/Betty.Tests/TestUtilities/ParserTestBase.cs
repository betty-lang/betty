namespace Betty.Tests.TestUtilities
{
    public class ParserTestBase
    {
        protected static Parser SetupParser(string code)
        {
            var lexer = new Lexer(code);
            var tokens = lexer.GetTokens();
            return new Parser(tokens);
        }
    }
}