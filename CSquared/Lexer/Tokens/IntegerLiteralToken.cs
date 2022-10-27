using System.Numerics;

namespace CSquared.Lexer.Tokens;

public record IntegerLiteralToken(BigInteger Value, Position From, Position To) : Token(TokenType.IntegerLiteral, From, To);