using System.Text.RegularExpressions;
using QParser.Lexer;

namespace QParser;

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
        var tokenTypeStr = TokenType switch
        {
            TokenConstants.Eof => "EOF",
            TokenConstants.Epsilon => "ϵ",
            TokenConstants.Unknown => "?",
            _ => ((DefaultTokenType)TokenType).ToString()
        };
        return $"[{tokenTypeStr} '{Regex.Escape(Content)}' ({SourceRange})]{(Ignore ? "?" : "")}";
    }
}