using System.Text;

namespace QParser.Parser;

public record ClosureItem(Rule Rule, CompositeNonterminal Production, int Index)
{
    public Nonterminal? AfterDot => Index >= Production.Components.Length ? null : Production.Components[Index];
    public Nonterminal? BeforeDot => Index == 0 ? null : Production.Components[Index - 1];

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(Rule.Name).Append(" -> ");
        for (var i = 0; i < Production.Components.Length; i++)
        {
            if (i == Index) sb.Append(". ");
            sb.Append(Production.Components[i].Name);
            if (i != Production.Components.Length - 1) sb.Append(' ');
        }

        if (Index == Production.Components.Length) sb.Append(" .");
        return sb.ToString();
    }
}