namespace QParser.Lexer.Tokens;

public class IntegerToken : Token
{
    public int Value;

    public IntegerToken(string content) : base((int)DefaultTokenType.Integer, content)
    {
        Value = int.Parse(content);
    }
}