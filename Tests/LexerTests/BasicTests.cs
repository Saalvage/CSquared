using CSquared.Lexer;

namespace Tests;

public class BasicTests {
    [Fact]
    public void TestComment() {
        Lexer lexer = new(" // */ *D /* == != === \n\r  \r\n");
        var tokens = lexer.Lex();
        Assert.Empty(tokens);
    }
}
