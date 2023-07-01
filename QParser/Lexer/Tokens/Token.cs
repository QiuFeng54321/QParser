using System.Text.RegularExpressions;

namespace QParser.Lexer.Tokens;

public class Token
{
    public TokenType TokenType;
    public readonly string Content;
    public CharPosition Start, End;
    public bool Ignore;

    public Token(TokenType tokenType, string content)
    {
        TokenType = tokenType;
        Content = content;
    }

    public override string ToString()
    {
        return $"[{TokenType} '{Regex.Escape(Content)}' ({Start} ~ {End})]{(Ignore ? "?" : "")}";
    }
}