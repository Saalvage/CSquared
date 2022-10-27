namespace CSquared.Lexer.Tokens;

public record StringLiteralToken(string Value, Position From, Position To) : Token(TokenType.StringLiteral, From, To);
