namespace Betty.Tests.LexerTests
{
    public class GeneralTests
    {
        [Fact]
        public void GetNextToken_ReturnsEOFForEmptyInput()
        {
            var lexer = new Lexer("");

            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.EOF, token.Type);
        }

        [Fact]
        public void GetNextToken_ParsesSequenceOfTokensCorrectly()
        {
            // Arrange
            var input = "x = 10;";
            var lexer = new Lexer(input);
            var expectedTokens = new List<TokenType> {
                TokenType.Identifier, TokenType.Equal,
                TokenType.NumberLiteral, TokenType.Semicolon,
                TokenType.EOF };

            // Act
            var tokens = new List<TokenType>();
            Token token;
            do
            {
                token = lexer.GetNextToken();
                tokens.Add(token.Type);
            } while (token.Type != TokenType.EOF);

            // Assert
            Assert.Equal(expectedTokens, tokens);
        }

        [Fact]
        public void PeekNextToken_ReturnsNextTokenWithoutAdvancingPosition()
        {
            // Arrange
            var input = "x = 10;";
            var lexer = new Lexer(input);

            // Act
            var peekedToken = lexer.PeekNextToken();
            var nextToken = lexer.GetNextToken();

            // Assert
            Assert.Equal(peekedToken.Type, nextToken.Type);
        }
    }
}