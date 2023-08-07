namespace QParser.Parser;

public class LR0ClosureTable : ClosureTable
{
    public LR0ClosureTable(Rule startingRule)
    {
        var startingClosureItem = new ClosureItem(startingRule, startingRule.SubRules.First(), 0);
        var startingKernels = new ClosureItemSet { startingClosureItem };
        StartingClosure = new Closure(NewId, startingKernels);
        RegisterClosure(StartingClosure);
    }


    /// <summary>
    ///     Generates the full closure table, containing CLOSURE and GOTO
    /// </summary>
    public override void Generate()
    {
        GenerateGoto(StartingClosure);
    }

    /// <summary>
    ///     Advances each item in the closure by 1 index, and generates GOTO(item)
    /// </summary>
    /// <param name="closure">The closure containing the items to generate GOTO for</param>
    private void GenerateGoto(Closure closure)
    {
        Dictionary<Nonterminal, ClosureItemSet> itemsToAdd = new();
        // We have to separately maintain a list to add, because closure items can change when we
        // call AddKernelItem
        foreach (var item in closure.Kernels.Concat(closure.NonKernels))
        {
            if (item.AfterDot is null)
            {
                FinishedItems.Add((closure.Id, item));
                continue;
            }

            var newItem = item with { Index = item.Index + 1 };
            itemsToAdd.TryAdd(item.AfterDot, new ClosureItemSet());

            itemsToAdd[item.AfterDot].Add(newItem);
        }

        foreach (var (nonterminal, items) in itemsToAdd)
        {
            AddKernelItem(items, out var newClosure, out var closureNew);
            GotoTable[(closure.Id, nonterminal)] = newClosure;
            if (closureNew) GenerateGoto(newClosure);
        }
    }

    public void Dump()
    {
        foreach (var (kernels, closure) in Closures) Console.WriteLine(closure);

        foreach (var ((id, nonterminal), closure) in GotoTable)
            Console.WriteLine($"GOTO(I{id}, {nonterminal}) = {closure.Id}");
    }
}