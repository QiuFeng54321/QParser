namespace QParser.Parser;

public class CompositeSubRule : SubRule
{
    public readonly SubRule[] Components;

    public CompositeSubRule(Grammar grammar, IEnumerable<SubRule> components) : this(grammar, components.ToArray()) {}

    public CompositeSubRule(Grammar grammar, params SubRule[] components) : base(grammar)
    {
        Components = components;
    }
    public static CompositeSubRule operator +(CompositeSubRule subRule1, CompositeSubRule subRule2)
    {
        return new CompositeSubRule(subRule1.Grammar, subRule1.Components.Concat(subRule2.Components));
    }

    public override string ToString()
    {
        return string.Join(" ", Components.Select(c => c.Name));
    }
}