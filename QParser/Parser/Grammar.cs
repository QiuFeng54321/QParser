using System.Text;

namespace QParser.Parser;

public class Grammar
{
    public readonly List<Rule> Rules = new();
    public void AddRule(Rule rule) => Rules.Add(rule);
    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var rule in Rules)
        {
            rule.Format(sb);
        }

        return sb.ToString();
    }
}