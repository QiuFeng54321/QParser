using System.Text.RegularExpressions;

namespace QParser.Lexer.Tokens;

public class Token
{
    public readonly string Content;
    public bool Ignore;
    public CharPosition Start, End;
    public TokenType TokenType;

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