using QParser.Lexer;

namespace QParser.Parser;

public class CompositeNonterminal : Nonterminal
{
    public readonly Nonterminal[] Components;

    public CompositeNonterminal(Grammar grammar, IEnumerable<Nonterminal> components) : this(grammar, components.ToArray()) {}

    public CompositeNonterminal(Grammar grammar, params Nonterminal[] components) : base(grammar)
    {
        Components = components;
    }
    public static CompositeNonterminal operator +(CompositeNonterminal subRule1, CompositeNonterminal subRule2)
    {
        return new CompositeNonterminal(subRule1.Grammar, subRule1.Components.Concat(subRule2.Components));
    }

    protected internal override void InternalGenerateFirstGenerator()
    {
        foreach (var component in Components)
        {
            FirstGenerator.Add(component);
            if (!component.CanBeEmpty) return;
        }

        First.Add(TokenType.Epsilon);
    }

    internal override void InternalGenerateCanBeEmpty()
    {
        if (Components.Any(component => !component.CanBeEmpty))
        {
            CanBeEmpty = false;
            return;
        }

        CanBeEmpty = true;
    }

    public override string ToString()
    {
        return string.Join(" ", Components.Select(c => c.Name));
    }
}