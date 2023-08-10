using System;
using System.Collections.Generic;
using System.Linq;
using QParser.Lexer;

namespace QParser.Parser;

public class CompositeNonterminal : Nonterminal
{
    public readonly Nonterminal[] Components;

    public CompositeNonterminal(Grammar grammar, IEnumerable<Nonterminal> components) : this(grammar,
        components.ToArray())
    {
    }

    public CompositeNonterminal(Grammar grammar, params Nonterminal[] components) : base(grammar)
    {
        if (components.Length == 0)
            throw new InvalidOperationException("Redundant composite rule. For epsilon, use rule.Add(Epsilon)");
        Components = components;
    }


    /// <summary>
    ///     This composite non-terminal contains only one token
    /// </summary>

    public bool IsSingleToken => Components.Length == 1 && Components[0] is TokenTerminal;

    /// <summary>
    ///     If this composite non-terminal contains only one token, returns the only token
    /// </summary>
    /// <exception cref="InvalidOperationException">There is no token / the first terminal is not token</exception>
    public TokenTerminal SingleToken => Components[0] as TokenTerminal ?? throw new InvalidOperationException();

    /// <summary>
    ///     This composite non-terminal contains only one rule
    /// </summary>
    public bool IsSingleRule => Components.Length == 1 && Components[0] is Rule;

    public static CompositeNonterminal operator +(CompositeNonterminal subRule1, CompositeNonterminal subRule2)
    {
        return new CompositeNonterminal(subRule1.Grammar, subRule1.Components.Concat(subRule2.Components));
    }

    protected override void InternalGenerateFirstGenerator()
    {
        if (IsSingleToken)
        {
            First.UnionWith(SingleToken.First);
        }
        else
        {
            foreach (var component in Components)
            {
                FirstGenerator.Add(component);
                if (!component.CanBeEmpty) return;
            }

            First.Add(TokenConstants.Epsilon);
        }
    }

    public override bool GenerateNonKernelItems()
    {
        var prevLen = NonKernelItems.Count;
        foreach (var component in Components)
        {
            NonKernelItems.UnionWith(component.NonKernelItems);
            if (!CanBeEmpty) break;
        }

        return prevLen != NonKernelItems.Count;
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