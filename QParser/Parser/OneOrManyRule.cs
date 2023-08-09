namespace QParser.Parser;

public class OneOrManyRule : Rule
{
    public OneOrManyRule(Grammar grammar, string name, HashSet<CompositeNonterminal> subRules, bool isGenerated = false)
        : base(grammar, name, subRules, isGenerated)
    {
    }

    public override ParseTreeNode MakeNode(CompositeNonterminal production, List<ParseTreeNode> children)
    {
        return new OneOrManyParseTreeNode(this, production, children);
    }
}