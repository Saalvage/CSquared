using CSquared.Lexer.Tokens;
using CSquared.Lexer;

namespace Tests.LexerTests {
    public class NumberTests {
        private static void TestSingleToken(string code, double number) {
            Lexer lexer = new(code);
            var tokens = lexer.Lex();
            Assert.Single(tokens);
            Assert.Equal(TokenType.FloatLiteral, tokens[0].Type);
            Assert.Equal(number, ((FloatLiteralToken)tokens[0]).Value);
        }

        private static void TestSingleToken(string code, int number) {
            Lexer lexer = new(code);
            var tokens = lexer.Lex();
            Assert.Single(tokens);
            Assert.Equal(TokenType.IntegerLiteral, tokens[0].Type);
            Assert.Equal(number, ((IntegerLiteralToken)tokens[0]).Value);
        }

        [Fact]
        public void TestHex() => TestSingleToken("0x0abcd", 0x0abcd);

        [Fact]
        public void TestHexWhitespace() => TestSingleToken("   -0xa b c d \t   \n\r\n\r // d", -0xabcd);

        [Fact]
        public void TestBinary() => TestSingleToken("0x0001", 1);

        [Fact]
        public void TestOctal() => TestSingleToken("0o173", 123);

        [Fact]
        public void TestDecimal() => TestSingleToken("123", 123);

        [Fact]
        public void TestDecimalLeadingZero() => TestSingleToken("01", 1);

        [Fact]
        public void LeadingPlus() => TestSingleToken("+123", 123);

        [Fact]
        public void LeadingDot() => TestSingleToken(".1", .1);

        [Fact]
        public void LeadingDotHex() => TestSingleToken("0x.1", 1.0 / 16);

        [Fact]
        public void LeadingDotWithSpaces() => TestSingleToken("0x. 1", 1.0 / 16);

        [Fact]
        public void TrailingDot() => TestSingleToken("-0x00.", 0.0);

        [Fact]
        public void ExplicitDecimal() => TestSingleToken("-0d99", -99);
    }
}