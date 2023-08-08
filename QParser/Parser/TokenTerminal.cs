using QParser.Lexer;

namespace QParser.Parser;

public class TokenTerminal : Nonterminal
{
    public readonly int TokenType;

    public TokenTerminal(Grammar grammar, int tokenType) : base(grammar)
    {
        TokenType = tokenType;
        Name = TokenType is TokenConstants.Epsilon ? "ϵ" : TokenType.ToString();
        // FIRST and CanBeEmpty can always be directly deduced independently
        First.Add(TokenType);
        CanBeEmpty = TokenType is TokenConstants.Epsilon;
        CanBeEmptyGenerated = true;
    }

    protected override void InternalGenerateFirstGenerator()
    {
    }

    public override bool GenerateNonKernelItems()
    {
        return false;
    }

    protected override void InternalGenerateCanBeEmpty()
    {
    }
}