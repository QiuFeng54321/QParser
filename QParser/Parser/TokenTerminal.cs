using QParser.Lexer;

namespace QParser.Parser;

public class TokenTerminal : Nonterminal
{
    public readonly TokenType TokenType;

    public TokenTerminal(Grammar grammar, TokenType tokenType) : base(grammar)
    {
        TokenType = tokenType;
        Name = TokenType is TokenType.Epsilon ? "ϵ" : TokenType.ToString();
        // FIRST and CanBeEmpty can always be directly deduced independently
        First.Add(TokenType);
        CanBeEmpty = TokenType is TokenType.Epsilon;
        CanBeEmptyGenerated = true;
    }

    protected internal override void InternalGenerateFirstGenerator()
    {
    }

    protected override void InternalGenerateCanBeEmpty()
    {
    }
}