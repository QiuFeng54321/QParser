namespace QParser.Generator.ParserAst;

public class ParserMacro : ParserStatement
{
    public ParserMacro(GrammarContext grammarContext, Token nameToken, Token valueToken) : base(grammarContext)
    {
        NameToken = nameToken;
        ValueToken = valueToken;
        if (NameToken.Content == "Tokens") grammarContext.FillTokens(ValueToken);
    }

    public Token NameToken { get; set; }
    public Token ValueToken { get; set; }
}