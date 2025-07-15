namespace Betty.Tests.LexerTests
{
    public class LiteralTests
    {
        [Theory]
        [InlineData(@"'a'", "a")]
        [InlineData(@"'\n'", "\n")]
        [InlineData(@"'\t'", "\t")]
        [InlineData(@"'\\'", "\\")]
        [InlineData(@"'1'", "1")]
        [InlineData(@"' '", " ")]
        [InlineData(@"'\''", "'")]
        public void GetNextToken_HandlesCharLiteralsCorrectly(string input, string expectedValue)
        {
            var lexer = new Lexer(input);

            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.CharLiteral, token.Type);
            Assert.Equal(expectedValue, token.Value!.ToString());
        }

        [Fact]
        public void GetNextToken_ThrowsExceptionForEmptyCharLiteral()
        {
            // Arrange
            var input = "''";
            var lexer = new Lexer(input);

            // Act & Assert
            Assert.Throws<Exception>(() => lexer.GetNextToken());
        }

        [Fact]
        public void GetNextToken_ThrowsExceptionForUnterminatedCharLiteral()
        {
            // Arrange
            var input = "'a";
            var lexer = new Lexer(input);

            // Act & Assert
            Assert.Throws<Exception>(() => lexer.GetNextToken());
        }

        [Theory]
        [InlineData("123", 123)]
        [InlineData("0", 0)]
        [InlineData("123.456", 123.456)]
        [InlineData(".789", 0.789)]
        [InlineData("0.1", 0.1)]
        public void GetNextToken_HandlesNumberLiteralsCorrectly(string input, double expectedValue)
        {
            var lexer = new Lexer(input);

            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.NumberLiteral, token.Type);
            Assert.Equal(expectedValue, token.Value!);
        }

        [Theory]
        [InlineData(@"Hello", "Hello")]
        [InlineData(@"", "")]
        [InlineData(@"\\", "\\")]
        [InlineData(@"String with spaces", "String with spaces")]
        [InlineData(@"123", "123")]
        [InlineData(@"String with \""escaped quotes\""", "String with \"escaped quotes\"")]
        [InlineData(@"Newline\nCharacter", "Newline\nCharacter")]
        [InlineData(@"Tab\tCharacter", "Tab\tCharacter")]
        public void GetNextToken_HandlesStringLiteralsCorrectly(string input, string expectedValue)
        {
            var lexer = new Lexer($"\"{input}\"");

            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal(expectedValue, token.Value);
        }

        [Theory]
        [InlineData("true", TokenType.True)]
        [InlineData("false", TokenType.False)]
        public void GetNextToken_HandlesBooleanLiteralsCorrectly(string input, TokenType expectedTokenType)
        {
            var lexer = new Lexer(input);

            var token = lexer.GetNextToken();

            Assert.Equal(expectedTokenType, token.Type);
        }

        [Fact]
        public void ScanStringLiteral_ThrowsExceptionForUnterminatedString()
        {
            // Arrange
            var input = "\"Hello";
            var lexer = new Lexer(input);

            // Act & Assert
            Assert.Throws<Exception>(() => lexer.GetNextToken());
        }

        [Fact]
        public void ScanStringLiteral_ThrowsExceptionForUnrecognizedEscapeSequence()
        {
            // Arrange
            var input = "\"Hello\\x\"";
            var lexer = new Lexer(input);

            // Act & Assert
            Assert.Throws<Exception>(() => lexer.GetNextToken());
        }
    }
}