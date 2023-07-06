using QParser.Lexer;

namespace QParser.Parser;

public class CompositeNonterminal : Nonterminal
{
    public readonly Nonterminal[] Components;

    public bool IsSingleToken => Components is [TokenTerminal];
    public bool IsSingleRule => Components is [Rule];

    public CompositeNonterminal(Grammar grammar, IEnumerable<Nonterminal> components) : this(grammar, components.ToArray()) {}

    public CompositeNonterminal(Grammar grammar, params Nonterminal[] components) : base(grammar)
    {
        if (components.Length == 0)
        {
            throw new InvalidOperationException("Redundant composite rule. For epsilon, use rule.Add(Epsilon)");
        }
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

    protected override void InternalGenerateCanBeEmpty()
    {
        foreach (var component in Components)
        {
            if (component.CanBeEmpty) continue;
            CanBeEmpty = false;
            // One component is definitely not empty => This cannot be empty
            CanBeEmptyGenerated = component.CanBeEmptyGenerated;
            return;
        }

        // All components may be empty
        // CanBeEmpty is set true if a token is epsilon.
        // This spreads like wildfire and is certain to be definitely generated.
        CanBeEmpty = CanBeEmptyGenerated = true;
    }

    public override string ToString()
    {
        return string.Join(" ", Components.Select(c => c.Name));
    }
}