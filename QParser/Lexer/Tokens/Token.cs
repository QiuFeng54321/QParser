using System.Text.RegularExpressions;

namespace QParser.Lexer.Tokens;

public class Token
{
    public readonly string Content;
    public readonly SourceRange SourceRange = new(new CharPosition(), new CharPosition());
    public bool Ignore;
    public int TokenType;

    public Token(int tokenType, string content)
    {
        TokenType = tokenType;
        Content = content;
    }

    public override string ToString()
    {
        return $"[{TokenType} '{Regex.Escape(Content)}' ({SourceRange})]{(Ignore ? "?" : "")}";
    }
}