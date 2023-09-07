namespace QParser.Generator.ParserAst;

public class ParserTokenDeclaration : ParserStatement
{
    public int Id;

    public ParserTokenDeclaration(GrammarContext grammarContext, Token nameToken, Token idToken) : base(grammarContext)
    {
        NameToken = nameToken;
        IdToken = idToken;
        grammarContext.Tokens.TryAdd(NameToken.Content, int.Parse(IdToken.Content));
    }

    public Token NameToken { get; set; }
    public Token IdToken { get; set; }
}