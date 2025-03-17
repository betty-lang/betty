
namespace Betty.Tests.LexerTests
{
    public class OperatorTests
    {
        [Theory]
        [InlineData("+", TokenType.Plus)]
        [InlineData("-", TokenType.Minus)]
        [InlineData("*", TokenType.Mul)]
        [InlineData("/", TokenType.Div)]
        [InlineData("%", TokenType.Mod)]
        [InlineData("=", TokenType.Equal)]
        [InlineData("==", TokenType.EqualEqual)]
        [InlineData("!=", TokenType.NotEqual)]
        [InlineData("<", TokenType.LessThan)]
        [InlineData(">", TokenType.GreaterThan)]
        [InlineData("<=", TokenType.LessThanOrEqual)]
        [InlineData(">=", TokenType.GreaterThanOrEqual)]
        [InlineData("&&", TokenType.And)]
        [InlineData("||", TokenType.Or)]
        [InlineData("!", TokenType.Not)]
        [InlineData("++", TokenType.Increment)]
        [InlineData("--", TokenType.Decrement)]
        [InlineData("+=", TokenType.PlusEqual)]
        [InlineData("-=", TokenType.MinusEqual)]
        [InlineData("*=", TokenType.MulEqual)]
        [InlineData("/=", TokenType.DivEqual)]
        [InlineData("%=", TokenType.ModEqual)]
        [InlineData("^", TokenType.Caret)]
        [InlineData("^=", TokenType.CaretEqual)]
        [InlineData("//", TokenType.IntDiv)]
        [InlineData("//=", TokenType.IntDivEqual)]
        [InlineData("..", TokenType.DotDot)]
        public void GetNextToken_HandlesOperatorsCorrectly(string input, TokenType expectedTokenType)
        {
            var lexer = new Lexer(input);

            var token = lexer.GetNextToken();

            Assert.Equal(expectedTokenType, token.Type);
        }
    }
}