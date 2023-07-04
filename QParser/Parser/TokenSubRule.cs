using System.Text;
using QParser.Lexer;

namespace QParser.Parser;

public class TokenSubRule : SubRule
{
    public TokenType TokenType;

    public TokenSubRule(Grammar grammar, TokenType tokenType) : base(grammar)
    {
        TokenType = tokenType;
        Name = TokenType.ToString();
    }
}