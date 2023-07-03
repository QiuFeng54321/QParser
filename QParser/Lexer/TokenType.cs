namespace QParser.Lexer;

public enum TokenType
{
    Identifier,
    Integer,
    Real,
    Dot,
    Indent,
    Dedent,
    Declare,
    As,
    If,
    Else,
    Then,
    For,
    To,
    Assignment,
    LessThan,
    GreaterThan,
    LessThanOrEqualTo,
    GreaterThanOrEqualTo,
    Equal,
    NotEqual,
    OpenParen,
    CloseParen,
    OpenBracket,
    CloseBracket,
    OpenBrace,
    CloseBrace,
    Eof,
    Space,
    Unknown
}