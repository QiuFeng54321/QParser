namespace QParser.Parser;

public class ClosureTable
{
    private readonly Dictionary<HashSet<ClosureItem>, Closure>
        _closures = new(HashSet<ClosureItem>.CreateSetComparer());

    private readonly Dictionary<(int id, Nonterminal nonterminal), Closure> _gotoTable = new();

    private readonly Dictionary<int, Closure> _idClosures = new();
    private readonly Closure _startingClosure;
    private int _currentId;

    public ClosureTable(Rule startingRule)
    {
        var startingClosureItem = new ClosureItem(startingRule, startingRule.SubRules.First(), 0);
        var startingKernels = new HashSet<ClosureItem> { startingClosureItem };
        _startingClosure = new Closure(NewId, startingKernels);
        RegisterClosure(_startingClosure);
    }

    private int NewId => _currentId++;

    private void RegisterClosure(Closure closure)
    {
        _closures[closure.Kernels] = closure;
        _idClosures[closure.Id] = closure;
    }

    /// <summary>
    ///     Adds an item to the closure table.
    ///     This item will be considered kernel, which means the (non-)terminal before the dot of this item
    ///     will be used as an index to the closure.
    ///     If the item cannot be put in any existing closures, a new closure will be created.
    /// </summary>
    /// <param name="kernelItems">The item to be added</param>
    /// <param name="closure">The closure which the item is put in</param>
    /// <param name="isClosureNew">Indicates if the closure was created after this call</param>
    private void AddKernelItem(HashSet<ClosureItem> kernelItems, out Closure closure, out bool isClosureNew)
    {
        isClosureNew = false;

        if (_closures.TryGetValue(kernelItems, out var foundClosure))
        {
            closure = foundClosure;
            return;
        }

        closure = new Closure(NewId, kernelItems);
        RegisterClosure(closure);
        isClosureNew = true;
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
        Dictionary<Nonterminal, HashSet<ClosureItem>> itemsToAdd = new();
        // We have to separately maintain a list to add, because closure items can change when we
        // call AddKernelItem
        foreach (var item in closure.Kernels.Concat(closure.NonKernels))
        {
            if (item.AfterDot is null) continue;
            var newItem = item with { Index = item.Index + 1 };
            if (!itemsToAdd.ContainsKey(item.AfterDot)) itemsToAdd.Add(item.AfterDot, new HashSet<ClosureItem>());

            itemsToAdd[item.AfterDot].Add(newItem);
        }

        foreach (var (nonterminal, items) in itemsToAdd)
        {
            AddKernelItem(items, out var newClosure, out var closureNew);
            _gotoTable[(closure.Id, nonterminal)] = newClosure;
            if (closureNew) GenerateGoto(newClosure);
        }
    }

    public void Dump()
    {
        foreach (var (kernels, closure) in _closures)
        {
            // Console.Write($"After {nonterminal}: ");
            Console.WriteLine(closure);
        }

        foreach (var ((id, nonterminal), closure) in _gotoTable)
            Console.WriteLine($"GOTO(I{id}, {nonterminal}) = {closure.Id}");
    }
}