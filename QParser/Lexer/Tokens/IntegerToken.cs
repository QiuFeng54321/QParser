namespace QParser.Lexer.Tokens;

public class IntegerToken : Token
{
    public int Value;

    public IntegerToken(TokenType tokenType, string content) : base(tokenType, content)
    {
        Value = int.Parse(content);
    }
}