using System.Text.RegularExpressions;

namespace QParser.Lexer.Tokens;

public class Token
{
    public readonly string Content;
    public readonly SourceRange SourceRange = new(new CharPosition(), new CharPosition());
    public bool Ignore;
    public TokenType TokenType;

    public Token(TokenType tokenType, string content)
    {
        TokenType = tokenType;
        Content = content;
    }

    public override string ToString()
    {
        return $"[{TokenType} '{Regex.Escape(Content)}' ({SourceRange})]{(Ignore ? "?" : "")}";
    }
}