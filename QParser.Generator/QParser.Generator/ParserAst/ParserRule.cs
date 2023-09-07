namespace QParser.Generator.ParserAst;

public class ParserRule : ParserSymbol
{
    public ParserRule(GrammarContext grammarConstructor, Token nameToken) : base(grammarConstructor,
        grammarConstructor.GrammarConstructor.R(nameToken.Content))
    {
        NameToken = nameToken;
    }

    public Token NameToken { get; set; }
}