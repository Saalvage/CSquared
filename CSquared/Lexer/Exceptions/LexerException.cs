namespace CSquared.Lexer.Exceptions;

public class LexerException : Exception {
    public override string Message { get; }
    private Position Start { get; }
    private Position? End { get; }

    public LexerException(string message, Position start) {
        Message = message;
        Start = start;
    }

    public LexerException(string message, Position start, Position end) {
        Message = message;
        Start = start;
        End = end;
    }

    public override string ToString() // TODO: Include source and cut double line number
        => $"{GetType().FullName}: {Message} at {Start}{(End is not null ? $" to {End}" : "")}";
}
