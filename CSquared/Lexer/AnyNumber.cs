using System.Numerics;
using CSquared.Lexer.Tokens;

namespace CSquared.Lexer;

public class AnyNumber {
    private readonly BigInteger _int;
    private readonly double _double;
    private readonly bool _isDouble;

    public AnyNumber(BigInteger i) {
        _int = i;
        _isDouble = false;
    }

    public AnyNumber(double d) {
        _double = d;
        _isDouble = true;
    }

    public static AnyNumber operator-(AnyNumber self)
        => self._isDouble ? new(-self._double) : new(-self._int);

    public Token ToToken(Position from, Position to)
        => _isDouble ? new FloatLiteralToken(_double, from, to) : new IntegerLiteralToken(_int, from, to);
}
