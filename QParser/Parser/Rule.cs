using System.Text;

namespace QParser.Parser;

public class Rule : Nonterminal
{
    private readonly HashSet<Nonterminal> _subRules;

    protected internal override void InternalGenerateFirstGenerator()
    {
        foreach (var subRule in _subRules)
        {
            subRule.GenerateFirstGenerator();
            if (subRule is TokenTerminal)
            {
                First.UnionWith(subRule.First);
            }
            else
            {
                FirstGenerator.Add(subRule);
            }
        }
    }

    protected override void InternalGenerateCanBeEmpty()
    {
        if (CanBeEmptyGenerated) return;
        if (_subRules.Contains(Grammar.Epsilon))
        {
            CanBeEmpty = CanBeEmptyGenerated = true;
            return;
        }

        foreach (var subRule in _subRules)
        {
            subRule.GenerateCanBeEmpty();
        }
        foreach (var r in _subRules)
        {
            if (!r.CanBeEmpty) continue;
            // A sub-rule is certain to allow empty 
            CanBeEmpty = CanBeEmptyGenerated = true;
            break;
        }
    }

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
        sb.AppendLine();
        sb.Append($"Rule {Name}");
        if (CanBeEmpty)
        {
            sb.Append(" (Can be empty)");
        }

        sb.Append(": ");
        if (withFirst)
        {
            sb.Append($" FIRST = {{{string.Join(", ", First)}}}");
        }

        sb.AppendLine();
        foreach (var subRule in _subRules)
        {
            sb.Append($"{Name} -> {subRule};");
            if (withFirst)
            {
                sb.Append($" FIRST = {{{string.Join(", ", subRule.First)}}}");
            }
            if (subRule.CanBeEmpty)
            {
                sb.Append(" (Can be empty)");
            }

            sb.AppendLine();
        }
    }
}