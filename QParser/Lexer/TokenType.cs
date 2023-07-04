namespace QParser.Lexer;

public enum TokenType
{
    Identifier,
    Integer,
    Real,
    Dot,
    Colon,
    Comma,
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
    Plus,
    Minus,
    Multiply,
    Divide,
    IntDivide,
    Power,
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