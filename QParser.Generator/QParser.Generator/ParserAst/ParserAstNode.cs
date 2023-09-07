namespace QParser.Generator.ParserAst;

public class ParserAstNode
{
    public ParserAstNode(GrammarContext grammarContext)
    {
        GrammarContext = grammarContext;
    }

    public GrammarContext GrammarContext { get; set; }
}