namespace QParser.Lexer.Tokens;

public class IdentifierToken : Token
{
    public IdentifierToken(string content) : base(TokenType.Identifier, content)
    {
    }
}