using System.Numerics;
using CSquared.Lexer.Exceptions;
using CSquared.Lexer.Tokens;

namespace CSquared.Lexer; 

public class Lexer {
    private Cursor _pos;
    
    public Lexer(string code) {
        _pos = new(code);
    }

    public Token[] Lex() {
        List<Token> tokens = new();
        while (LexOne() is { } t) {
            tokens.Add(t);
        }
        return tokens.ToArray();
    }

    public Token? LexOne() {
        while (_pos.IsValid()) {
            // Comment
            if (_pos == '/' && _pos.TryGetNext() == '/') {
                _pos++; _pos++;
                while (_pos.IsValid() && (char)_pos is not ('\n' or '\r')) {
                    _pos++;
                }

                if (_pos.IsValid()) {
                    _pos++;
                }
                continue;
            }

            // Whitespace
            if (char.IsWhiteSpace(_pos)) {
                _pos++;
                continue;
            }

            // Number
            if (char.IsDigit(_pos) || TryParseSign(out _) || HasStartingDot()) {
                var startPos = _pos;

                if (TryParseSign(out var negative)) {
                    _pos++;
                }

                if (!HasStartingDot() && _pos == '0' && _pos.IsNextValid()) {
                    _pos++;

                    var num =
                        TryGetLiteralToken('x', 16, c => c switch {
                            >= '0' and <= '9' => c - '0',
                            >= 'a' and <= 'f' => c - 'a' + 10,
                            >= 'A' and <= 'F' => c - 'A' + 10,
                            _ => -1,
                        }) ??
                        TryGetLiteralToken('b', 2, c => c switch {
                            '0' => 0,
                            '1' => 1,
                            _ => -1,
                        }) ??
                        TryGetLiteralToken('o', 8, c => c switch {
                            >= '0' and < '8' => c - '0',
                            _ => -1,
                        });

                    if (num != null) {
                        return (negative ? -num : num).ToToken(startPos.Position, _pos.Position);
                    }

                    if (_pos == 'd') {
                        _pos++;
                    }
                }

                var value = GetLiteralToken(10, c => c switch {
                    >= '0' and <= '9' => c - '0',
                    _ => -1,
                });
                return (negative ? -value : value).ToToken(startPos.Position, _pos.Position);
            }

            if (_pos == '"') {
                _pos++;
                var startPos = _pos;
                var wasBackslash = false;
                while (_pos != '"' || wasBackslash) {
                    wasBackslash = _pos == '\\';
                    _pos++;
                }

                return new StringLiteralToken(startPos.To(_pos), startPos.Position, _pos.Position);
            }

            /*  TODO:
                - Identifiers
                - Char literal
             */

            return new((char)_pos switch {
                '(' => TokenType.ParenthesisLeft,
                ')' => TokenType.ParenthesisRight,
                '{' => TokenType.BraceLeft,
                '}' => TokenType.BraceRight,
                '[' => TokenType.BracketLeft,
                ']' => TokenType.BracketRight,
                '<' => TokenType.AngleLeft,
                '>' => TokenType.AngleRight,
                '.' => TokenType.Dot,
                ',' => TokenType.Comma,
                ':' => TokenType.Colon,
                ';' => TokenType.Semicolon,
                '+' => TokenType.Plus,
                '-' => TokenType.Minus,
                '*' => TokenType.Star,
                '/' => TokenType.Slash,
                '%' => TokenType.Percent,
                '!' => TokenType.Exclamation,
                '~' => TokenType.Tilde,
                '?' => TokenType.Question,
                '^' => TokenType.Caret,
                '&' => TokenType.And,
                '|' => TokenType.Or,
                '#' => TokenType.Hash,
                '$' => TokenType.Dollar,
                '=' => TokenType.Equal,
                '\\' => TokenType.BackSlash,
                _ => throw new LexerException("Unknown token", _pos.Position),
            }, _pos.Position, _pos.Position);
        }

        return null;
    }

    private AnyNumber GetLiteralToken(int toBase, Func<char, int> mapper) {
        BigInteger value = new();
        if (!HasStartingDot()) {
            // Skip leading zeros
            while (_pos.IsValid() && _pos == '0') {
                _pos++;
            }
            while (_pos.IsValid()) {
                if (char.IsWhiteSpace(_pos)) { } else if (mapper(_pos) is var v && v != -1) {
                    value *= toBase;
                    value += v;
                } else {
                    break;
                }
                _pos++;
            }
        }

        if (_pos.IsValid() && _pos == '.') {
            _pos++;
            var value2 = (double)value;
            var divider = 1.0;
            while (_pos.IsValid()) {
                if (char.IsWhiteSpace(_pos)) { }
                else if (mapper(_pos) is var v && v != -1) {
                    divider /= toBase;
                    value2 += divider * v;
                } else {
                    break;
                }
                _pos++;
            }
            return new(value2);
        } else {
            return new(value);
        }
    }

    private AnyNumber? TryGetLiteralToken(char identifier, int toBase, Func<char, int> mapper) {
        if (char.ToLowerInvariant(_pos) == identifier && (mapper(_pos.TryGetNext()) != -1 || _pos.TryGetNext() == '.')) {
            _pos++;
            return GetLiteralToken(toBase, mapper);
        } else {
            return null;
        }
    }

    private bool TryParseSign(out bool negative) {
        negative = false;

        if (!char.IsDigit(_pos.TryGetNext())) {
            return false;
        }

        if ((char)_pos is '-' or '+') {
            negative = _pos == '-';
            return true;
        }

        return false;
    }

    private bool HasStartingDot()
        => _pos == '.' && char.IsDigit(_pos.TryGetNext());
}
