namespace QParser.Generator.ParserAst;

public class ParserToken : ParserSymbol
{
    public ParserToken(GrammarContext grammarContext, Token nameToken) : base(grammarContext,
        null!)
    {
        NameToken = nameToken;
        if (!grammarContext.Tokens.TryGetValue(nameToken.Content, out var tokenId))
        {
            new PrettyException(grammarContext.FileInformation, nameToken.SourceRange,
                "Unknown token" + nameToken.Content).AddToExceptions();
            tokenId = TokenConstants.Unknown;
        }

        Nonterminal = grammarContext.GrammarConstructor.T(tokenId);
    }

    public Token NameToken { get; set; }
}