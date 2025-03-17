namespace Betty.Tests.TestUtilities
{
    public class ParserTestBase
    {
        protected static Parser SetupParser(string code)
        {
            var lexer = new Lexer(code);
            return new Parser(lexer);
        }
    }
}