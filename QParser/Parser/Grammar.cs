using System.Text;
using QParser.Lexer;

namespace QParser.Parser;

public class Grammar
{
    public readonly HashSet<Rule> Rules = new();
    public void AddRule(Rule rule) => Rules.Add(rule);
    public readonly TokenTerminal Epsilon;
    public Rule? EntryRule { set; get; }
    public bool FirstGenerated;

    public Grammar()
    {
        Epsilon = new(this, TokenType.Epsilon);
    }

    public void GenerateCanBeEmpty()
    {
        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var rule in Rules)
            {
                changed |= rule.GenerateCanBeEmpty();
            }
        }
    }
    public void GenerateFirst()
    {
        if (FirstGenerated) return;
        var changed = true;
        foreach (var rule in Rules)
        {
            rule.GenerateFirstGenerator();
        }
        while (changed)
        {
            changed = false;
            foreach (var rule in Rules)
            {
                foreach (var subRule in rule.FirstGenerator)
                {
                    changed |= subRule.RoundGenerateFirst();
                }
                changed |= rule.RoundGenerateFirst();
            }
        }

        FirstGenerated = true;
    }

    public override string ToString()
    {
        return ToString(FirstGenerated);
    }

    public string ToString(bool withFirst)
    {
        var sb = new StringBuilder();
        foreach (var rule in Rules)
        {
            rule.Format(sb, withFirst);
        }

        return sb.ToString();
    }
}