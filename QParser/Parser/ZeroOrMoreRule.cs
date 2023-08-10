using System.Collections.Generic;

namespace QParser.Parser;

public class ZeroOrMoreRule : Rule
{
    public ZeroOrMoreRule(Grammar grammar, string name, HashSet<CompositeNonterminal> subRules,
        bool isGenerated = false) : base(grammar, name, subRules, isGenerated)
    {
    }

    public override ParseTreeNode MakeNode(CompositeNonterminal production, List<ParseTreeNode> children)
    {
        return new ZeroOrMoreParseTreeNode(this, production, children);
    }
}