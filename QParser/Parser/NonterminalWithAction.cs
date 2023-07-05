using System.Text;

namespace QParser.Parser;

public class NonterminalWithAction : Nonterminal
{
    public readonly Nonterminal InternalNonterminal;
    public readonly Action<Nonterminal>[] Actions;

    public NonterminalWithAction(Nonterminal internalNonterminal, IEnumerable<Action<Nonterminal>> actions) : this(internalNonterminal,
        actions.ToArray()){}
    public NonterminalWithAction(Nonterminal internalNonterminal, params Action<Nonterminal>[] actions) : base(internalNonterminal.Grammar)
    {
        InternalNonterminal = internalNonterminal;
        Actions = actions;
        Name = InternalNonterminal.Name;
    }
    public static NonterminalWithAction operator *(NonterminalWithAction nonterminal, Action<Nonterminal> action)
    {
        return new NonterminalWithAction(nonterminal, nonterminal.Actions.Append(action));
    }

    protected override void InternalGenerateFirst()
    {
        InternalNonterminal.GenerateFirst();
        foreach (var tokenType in InternalNonterminal.First)
        {
            First.Add(tokenType);
        }
    }

    public override bool CanBeEmpty => InternalNonterminal.CanBeEmpty;
    public override string ToString() => $"{InternalNonterminal} [#{Actions.Length}]";

}