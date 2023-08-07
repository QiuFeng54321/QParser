namespace QParser.Parser;

public abstract class ClosureTable
{
    protected readonly Dictionary<ClosureItemSet, Closure>
        Closures = new(ClosureItemSet.CreateSetComparer());

    public readonly HashSet<(int Id, ClosureItem item)> FinishedItems = new();

    public readonly Dictionary<(int id, Nonterminal symbol), Closure> GotoTable = new();

    private int _currentId;

    protected Closure StartingClosure = null!;

    protected int NewId => _currentId++;

    public abstract void Generate();

    protected void RegisterClosure(Closure closure)
    {
        Closures[closure.Kernels] = closure;
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
    protected void AddKernelItem(ClosureItemSet kernelItems, out Closure closure, out bool isClosureNew)
    {
        isClosureNew = false;

        if (Closures.TryGetValue(kernelItems, out var foundClosure))
        {
            closure = foundClosure;
            return;
        }

        closure = new Closure(NewId, kernelItems);
        RegisterClosure(closure);
        isClosureNew = true;
    }
}