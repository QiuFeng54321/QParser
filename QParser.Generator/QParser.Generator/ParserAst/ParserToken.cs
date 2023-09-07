namespace QParser.Generator.ParserAst;

public class ParserToken : ParserSymbol
{
    public ParserToken(GrammarContext grammarContext, Token nameToken) : base(grammarContext,
        grammarContext.GrammarConstructor.T(grammarContext.Tokens[nameToken.Content]))
    {
        NameToken = nameToken;
    }

    public Token NameToken { get; set; }
}