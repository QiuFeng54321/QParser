namespace QParser.Lexer.Tokens;

public class IntegerToken : Token
{
    public int Value;

    public IntegerToken(string content) : base(TokenType.Integer, content)
    {
        Value = int.Parse(content);
    }
}