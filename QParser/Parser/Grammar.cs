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

    public void GenerateFirst()
    {
        if (FirstGenerated) return;
        foreach (var rule in Rules)
        {
            rule.GenerateFirst();
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