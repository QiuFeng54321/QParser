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

    protected override void InternalGenerateFirst()
    {
        foreach (var component in Components)
        {
            component.GenerateFirst();
            First.UnionWith(component.First);
            if (!component.CanBeEmpty) return;
            First.Remove(TokenType.Epsilon);
        }

        First.Add(TokenType.Epsilon);
    }

    public override bool CanBeEmpty => Components.All(component => component.CanBeEmpty);

    public override string ToString()
    {
        return string.Join(" ", Components.Select(c => c.Name));
    }
}