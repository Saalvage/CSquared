namespace CSquared.Lexer.Tokens;

public record FloatLiteralToken(double Value, Position From, Position To) : Token(TokenType.FloatLiteral, From, To);
