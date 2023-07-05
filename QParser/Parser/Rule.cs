using System.Text;

namespace QParser.Parser;

public class Rule : Nonterminal
{
    public readonly HashSet<Nonterminal> SubRules;

    protected internal override void InternalGenerateFirstGenerator()
    {
        foreach (var subRule in SubRules)
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
        if (SubRules.Contains(Grammar.Epsilon))
        {
            CanBeEmpty = CanBeEmptyGenerated = true;
            return;
        }

        foreach (var subRule in SubRules)
        {
            subRule.GenerateCanBeEmpty();
        }
        foreach (var r in SubRules)
        {
            if (!r.CanBeEmpty) continue;
            // A sub-rule is certain to allow empty 
            CanBeEmpty = CanBeEmptyGenerated = true;
            break;
        }
    }

    public bool IsRedundant => SubRules.Count == 1;

    public Rule(Grammar grammar, string name, HashSet<Nonterminal> subRules) : base(grammar)
    {
        SubRules = subRules;
        Name = name;
    }
    public Rule Add(Nonterminal nonterminal)
    {
        SubRules.Add(nonterminal);
        return this;
    }
    

    public void Format(StringBuilder sb, bool withFirst, bool withFollow)
    {
        sb.AppendLine();
        sb.Append($"Rule {Name}");
        if (CanBeEmpty)
        {
            sb.Append(" (Can be empty)");
        }

        sb.Append(": ");
        AppendFirstFollow(sb, this, withFirst, withFollow);

        sb.AppendLine();
        foreach (var subRule in SubRules)
        {
            sb.Append($"{Name} -> {subRule};");
            AppendFirstFollow(sb, subRule, withFirst, withFollow);
            if (subRule.CanBeEmpty)
            {
                sb.Append(" (Can be empty)");
            }

            sb.AppendLine();
        }
    }

    private void AppendFirstFollow(StringBuilder sb, Nonterminal rule, bool withFirst, bool withFollow)
    {
        if (withFirst)
        {
            sb.Append($" FIRST = {{{string.Join(", ", rule.First)}}}");
        }
        if (withFollow)
        {
            sb.Append($" FOLLOW = {{{string.Join(", ", rule.Follow)}}}");
        }
    }
}