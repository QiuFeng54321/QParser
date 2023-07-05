using System.Text;
using QParser.Lexer;

namespace QParser.Parser;

public class TokenTerminal : Nonterminal
{
    public readonly TokenType TokenType;

    public TokenTerminal(Grammar grammar, TokenType tokenType) : base(grammar)
    {
        TokenType = tokenType;
        Name = TokenType is TokenType.Epsilon ? "ϵ" : TokenType.ToString();
    }

    protected override void InternalGenerateFirst()
    {
        // if (TokenType is not TokenType.Epsilon) 
        First.Add(TokenType);
    }

    public override bool CanBeEmpty => TokenType is TokenType.Epsilon;
}