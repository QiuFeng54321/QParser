using System.Collections.Generic;
using QParser.Parser;

namespace QParser.Generator.ParserAst;

public class ParserCompositeRule : ParserSymbol
{
    public List<ParserSymbol> Symbols;

    public ParserCompositeRule(GrammarContext grammarContext, List<ParserSymbol> symbols) : base(grammarContext, null!)
    {
        Symbols = symbols;
        var nonterminals = new Nonterminal[symbols.Count];
        for (var i = 0; i < symbols.Count; i++) nonterminals[i] = symbols[i].Nonterminal;
        Nonterminal = grammarContext.GrammarConstructor.C(nonterminals);
    }
}