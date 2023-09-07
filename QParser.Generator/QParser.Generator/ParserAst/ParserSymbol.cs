using QParser.Parser;

namespace QParser.Generator.ParserAst;

public class ParserSymbol : ParserAstNode
{
    public ParserSymbol(GrammarContext grammarContext, Nonterminal nonterminal) : base(grammarContext)
    {
        Nonterminal = nonterminal;
    }

    public Nonterminal Nonterminal { get; set; }
}