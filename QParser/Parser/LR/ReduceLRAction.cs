namespace QParser.Parser.LR;

public class ReduceLRAction : LRAction
{
    public CompositeNonterminal Production;
    public Rule Rule;

    public ReduceLRAction(Rule rule, CompositeNonterminal production)
    {
        Rule = rule;
        Production = production;
    }

    public override string ToString()
    {
        return $"r{{{Rule} -> {Production}}}";
    }
}