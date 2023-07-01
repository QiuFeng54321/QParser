namespace QParser.Lexer.Tokens;

public class Token
{
    public readonly TokenType TokenType = TokenType.Unknown;
    public readonly string Content;
    public CharPosition Start, End;
    public bool Ignore;

    public Token(string content, CharPosition start, CharPosition end, bool ignore)
    {
        Content = content;
        Start = start;
        End = end;
        Ignore = ignore;
    }

    public Token(TokenType tokenType, string content)
    {
        TokenType = tokenType;
        Content = content;
    }

    public override string ToString()
    {
        return $"[{TokenType} '{Content}' ({Start} ~ {End})]{(Ignore ? "?" : "")}";
    }
}