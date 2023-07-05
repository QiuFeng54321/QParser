using System.Text;

namespace QParser.Parser;

public class Rule : Nonterminal
{
    private readonly HashSet<Nonterminal> _subRules;
    protected override void InternalGenerateFirst()
    {
        foreach (var subRule in _subRules)
        {
            subRule.GenerateFirst();
            First.UnionWith(subRule.First);
        }
    }

    public override bool CanBeEmpty => _subRules.Any(r => r.CanBeEmpty);
    public bool IsRedundant => _subRules.Count == 1;

    public Rule(Grammar grammar, string name, HashSet<Nonterminal> subRules) : base(grammar)
    {
        _subRules = subRules;
        Name = name;
    }
    public Rule Add(Nonterminal nonterminal)
    {
        _subRules.Add(nonterminal);
        return this;
    }
    

    public void Format(StringBuilder sb, bool withFirst = false)
    {
        foreach (var subRule in _subRules)
        {
            sb.Append($"{Name} -> {subRule}");
            if (withFirst)
            {
                sb.Append($" {{{string.Join(", ", subRule.First)}}}");
            }

            sb.AppendLine();
        }
    }
}