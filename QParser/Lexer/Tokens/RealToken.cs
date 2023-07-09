namespace QParser.Lexer.Tokens;

public class RealToken : Token
{
    public double Value;

    public RealToken(string content) : base(TokenType.Real, content)
    {
        Value = double.Parse(content);
    }
}