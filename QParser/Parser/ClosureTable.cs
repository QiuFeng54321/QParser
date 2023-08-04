namespace QParser.Parser;

public class ClosureTable
{
    public readonly Dictionary<Nonterminal, Closure> ClosureAfter = new();
    public readonly Dictionary<ClosureItem, Closure> Closures = new();
    public readonly Closure StartingClosure;
    private int _currentId;

    public ClosureTable(Rule startingRule)
    {
        var startingClosureItem = new ClosureItem(startingRule, startingRule.SubRules.First(), 0);
        StartingClosure = new Closure(NewId, startingClosureItem);
        AddKernelItem(startingClosureItem, out _, out _);
    }

    public int NewId => _currentId++;

    public void AddKernelItem(ClosureItem item, out Closure closure, out bool isClosureNew)
    {
        isClosureNew = false;
        if (item.BeforeDot == null)
        {
            Closures[item] = StartingClosure;
            closure = StartingClosure;
            return;
        }

        if (Closures.TryGetValue(item, out var foundClosure))
        {
            closure = foundClosure;
            return;
        }

        if (!ClosureAfter.ContainsKey(item.BeforeDot))
        {
            closure = new Closure(NewId, item);
            ClosureAfter[item.BeforeDot] = closure;
            Closures[item] = closure;
            isClosureNew = true;
            return;
        }

        closure = ClosureAfter[item.BeforeDot];
        closure.Expand(item);
    }
}