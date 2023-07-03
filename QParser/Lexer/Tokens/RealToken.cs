namespace QParser.Lexer.Tokens;

public class RealToken : Token
{
    public double Value;
    public RealToken(TokenType tokenType, string content) : base(tokenType, content)
    {
        Value = double.Parse(content);
    }
}