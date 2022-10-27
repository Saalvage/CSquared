using CSquared.Lexer;

namespace Tests; 

public class CursorTests {
    [Fact]
    public void Increment() {
        var cur = new Cursor("abcdefghijklmnopqrstuvwxyz");
        Assert.Equal('a', cur);
        Assert.Equal('a', cur++);
        Assert.Equal('b', cur);
        Assert.Equal('c', ++cur);
    }
}
