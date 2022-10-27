namespace CSquared.Lexer;
    
public record struct Cursor(Position Position) {
    private readonly string _code = null!;

    public Cursor(string code) : this(default(Position)) {
        _code = code;
    }

    public bool IsValid() => Position.Index < _code.Length;

    public bool IsNextValid() => Position.Index < _code.Length - 1;

    public char TryGetNext() => IsNextValid() ? _code[Position.Index + 1] : '\0';

    public static Cursor operator++(Cursor cur) {
        if (!cur.IsValid()) {
            throw new InvalidOperationException("EOF");
        }

        var newPos = cur.Position;
        var ch = cur._code[newPos.Index];
        if (ch is '\n' or '\r') {
            var doubleSkip = false;
            if (cur.IsNextValid()) {
                var nextCh = cur._code[newPos.Index + 1];
                if (ch != nextCh && nextCh is '\n' or '\r') {
                    doubleSkip = true;
                }
            }

            newPos = new(newPos.Index + (doubleSkip ? 2 : 1), newPos.Line + 1, 1);
        } else {
            newPos = new(newPos.Index + 1, newPos.Line, newPos.Column + 1);
        }

        return cur with { Position = newPos };
    }

    public static Cursor operator+(Cursor cur, int length) {
        for (var i = 0; i < length; i++) {
            cur++;
        }

        return cur;
    }

    public static implicit operator char(Cursor pos) => pos._code[pos.Position.Index];

    public string To(Cursor other)
        => _code[Position.Index..other.Position.Index];
}
