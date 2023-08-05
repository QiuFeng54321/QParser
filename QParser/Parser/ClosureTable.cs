namespace QParser.Parser;

public class ClosureTable
{
    private readonly Dictionary<Nonterminal, Closure> _closureAfter = new();
    private readonly Dictionary<ClosureItem, Closure> _closures = new();
    private readonly Closure _startingClosure;
    private int _currentId;

    public ClosureTable(Rule startingRule)
    {
        var startingClosureItem = new ClosureItem(startingRule, startingRule.SubRules.First(), 0);
        _startingClosure = new Closure(NewId, startingClosureItem);
        AddKernelItem(startingClosureItem, out _, out _, out _);
    }

    private int NewId => _currentId++;

    /// <summary>
    ///     Adds an item to the closure table.
    ///     This item will be considered kernel, which means the (non-)terminal before the dot of this item
    ///     will be used as an index to the closure.
    ///     If the item cannot be put in any existing closures, a new closure will be created.
    /// </summary>
    /// <param name="item">The item to be added</param>
    /// <param name="closure">The closure which the item is put in</param>
    /// <param name="isClosureNew">Indicates if the closure was created after this call</param>
    /// <param name="closureChanged">Indicates if the items of the closure has changed</param>
    private void AddKernelItem(ClosureItem item, out Closure closure, out bool isClosureNew, out bool closureChanged)
    {
        isClosureNew = false;
        closureChanged = false;
        if (item.BeforeDot == null)
        {
            _closures[item] = _startingClosure;
            closure = _startingClosure;
            return;
        }

        if (_closures.TryGetValue(item, out var foundClosure))
        {
            closure = foundClosure;
            return;
        }

        if (!_closureAfter.ContainsKey(item.BeforeDot))
        {
            closure = new Closure(NewId, item);
            _closureAfter[item.BeforeDot] = closure;
            _closures[item] = closure;
            isClosureNew = true;
            closureChanged = true;
            return;
        }

        closure = _closureAfter[item.BeforeDot];
        closureChanged = closure.Expand(item);
    }

    /// <summary>
    ///     Generates the full closure table, containing CLOSURE and GOTO
    /// </summary>
    public void Generate()
    {
        GenerateGoto(_startingClosure);
    }

    /// <summary>
    ///     Advances each item in the closure by 1 index, and generates GOTO(item)
    /// </summary>
    /// <param name="closure">The closure containing the items to generate GOTO for</param>
    private void GenerateGoto(Closure closure)
    {
        HashSet<Closure> newClosures = new();
        HashSet<ClosureItem> itemsToAdd = new();
        // We have to separately maintain a list to add, because closure items can change when we
        // call AddKernelItem
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
        foreach (var (nonterminal, closure) in _closureAfter)
        {
            Console.Write($"After {nonterminal}: ");
            Console.WriteLine(closure);
        }
    }
}