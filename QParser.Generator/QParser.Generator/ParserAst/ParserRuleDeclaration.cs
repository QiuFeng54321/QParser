using System.Collections.Generic;
using QParser.Parser;

namespace QParser.Generator.ParserAst;

public class ParserRuleDeclaration : ParserStatement
{
    public readonly List<ParserCompositeRule> ParserCompositeRules;

    public ParserRuleDeclaration(GrammarContext grammarConstructor, List<ParserCompositeRule> parserCompositeRules,
        ParserRule name) : base(grammarConstructor)
    {
        ParserCompositeRules = parserCompositeRules;
        Name = name;
        HashSet<CompositeNonterminal> compositeNonterminals = new();
        foreach (var parserCompositeRule in ParserCompositeRules)
            compositeNonterminals.Add((CompositeNonterminal)parserCompositeRule.Nonterminal);
        Rule = GrammarContext.GrammarConstructor.R(name.NameToken.Content, false, compositeNonterminals);
    }

    public ParserRule Name { get; set; }
    public Rule Rule { get; set; }
}