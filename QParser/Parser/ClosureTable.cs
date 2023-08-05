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
        AddKernelItem(startingClosureItem, out _, out _, out _);
    }

    public int NewId => _currentId++;

    public void AddKernelItem(ClosureItem item, out Closure closure, out bool isClosureNew, out bool closureChanged)
    {
        isClosureNew = false;
        closureChanged = false;
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
            closureChanged = true;
            return;
        }

        closure = ClosureAfter[item.BeforeDot];
        closureChanged = closure.Expand(item);
    }

    public void Generate()
    {
        GenerateGoto(StartingClosure);
    }

    public void GenerateGoto(Closure closure)
    {
        HashSet<Closure> newClosures = new();
        HashSet<ClosureItem> itemsToAdd = new();
        foreach (var item in closure.Items)
        {
            if (item.AfterDot is null) continue;
            var newItem = item with { Index = item.Index + 1 };
            itemsToAdd.Add(newItem);
        }

        foreach (var item in itemsToAdd)
        {
            AddKernelItem(item, out var newClosure, out _, out var closureChanged);
            if (closureChanged) newClosures.Add(newClosure);
        }

        foreach (var newClosure in newClosures) GenerateGoto(newClosure);
    }

    public void Dump()
    {
        foreach (var (nonterminal, closure) in ClosureAfter)
        {
            Console.Write($"After {nonterminal}: ");
            Console.WriteLine(closure);
        }
    }
}