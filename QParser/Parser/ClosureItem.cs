namespace QParser.Parser;

public record ClosureItem(Rule Rule, CompositeNonterminal Production, int Index)
{
    public Nonterminal? AfterDot => Index >= Production.Components.Length ? null : Production.Components[Index];
    public Nonterminal? BeforeDot => Index == 0 ? null : Production.Components[Index - 1];
}