using System.Text;

namespace QParser.Parser;

public class Rule : SubRule
{
    public List<SubRule> SubRules;
    public bool CanBeEmpty { get; set; }

    public Rule(Grammar grammar, string name, List<SubRule> subRules) : base(grammar)
    {
        SubRules = subRules;
        Name = name;
    }

    public Rule AddEpsilon()
    {
        CanBeEmpty = true;
        return this;
    }
    public Rule Add(SubRule subRule)
    {
        SubRules.Add(subRule);
        return this;
    }

    public void Format(StringBuilder sb)
    {
        foreach (var subRule in SubRules)
        {
            sb.AppendLine($"{Name} -> {subRule}");
        }

        if (CanBeEmpty)
        {
            sb.AppendLine($"{Name} -> ϵ");
        }
    }
}