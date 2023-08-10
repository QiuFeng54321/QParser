using System;
using System.Collections.Generic;
using System.Linq;
using QParser.Lexer;

namespace QParser.Parser.LR;

public class BLRClosureTable : ClosureTable
{
    public readonly Dictionary<LookaheadIrrelevantClosureItemSet, HashSet<Closure>> MergeableClosures = new();

    public BLRClosureTable(Rule startingRule)
    {
        var startingClosureItem =
            new ClosureItem(
                startingRule,
                startingRule.SubRules.First(),
                0,
                TokenConstants.Eof);
        var startingKernels = new ClosureItemSet { startingClosureItem };
        StartingClosure = new Closure(NewId, startingKernels);
        RegisterClosure(StartingClosure);
    }

    public override void Generate()
    {
        GenerateGoto(StartingClosure);
    }

    /// <summary>
    ///     Adds an item to the closure table.
    ///     This item will be considered kernel, which means the (non-)terminal before the dot of this item
    ///     will be used as an index to the closure.
    ///     If the item cannot be put in any existing closures, a new closure will be created.
    /// </summary>
    /// <param name="primeItems">The item to be added</param>
    /// <param name="secondaryItems">Items without lookahead</param>
    /// <param name="closure">The closure which the item is put in</param>
    /// <param name="isClosureNew">Indicates if the closure was created after this call</param>
    protected void AddKernelItem(ClosureItemSet primeItems, LookaheadIrrelevantClosureItemSet secondaryItems,
        out Closure closure, out bool isClosureNew)
    {
        isClosureNew = false;

        if (Closures.TryGetValue(primeItems, out var foundClosure))
        {
            closure = foundClosure;
            return;
        }

        closure = new Closure(NewId, primeItems);
        if (MergeableClosures.TryGetValue(secondaryItems, out var mergeableClosures))
        {
            var merged = false;
            foreach (var mergeableClosure in mergeableClosures)
            {
                if (!Merge(mergeableClosure, closure)) continue;
                merged = true;
                break;
            }

            if (!merged) mergeableClosures.Add(closure);
        }
        else
        {
            MergeableClosures.Add(secondaryItems, new HashSet<Closure> { closure });
        }

        RegisterClosure(closure);
        isClosureNew = true;
    }

    /// <summary>
    ///     Backtracking: merge two closures and see if any r/r conflict occurs.
    /// </summary>
    /// <param name="closure"></param>
    /// <param name="newClosure"></param>
    private bool Merge(Closure closure, Closure newClosure)
    {
        ClosureItemSet kernelsToAdd = new(newClosure.Kernels);
        kernelsToAdd.ExceptWith(closure.Kernels);
        if (kernelsToAdd.Count == 0) return true;
        ClosureItemSet nonKernelsAdded = new(closure.NonKernels);
        closure.ExpandKernels(kernelsToAdd);
        nonKernelsAdded.SymmetricExceptWith(closure.NonKernels);
        // Test r/r conflict
        Dictionary<int, ClosureItem> finishedKernels = new();
        foreach (var kernel in closure.Kernels)
        {
            if (kernel.AfterDot is not null) continue;
            if (!finishedKernels.TryAdd(kernel.Lookahead, kernel)) goto revert;
        }
        // Try merge closures from non-kernels

        return true;
        revert:
        closure.Kernels.ExceptWith(kernelsToAdd);
        closure.NonKernels.ExceptWith(nonKernelsAdded);
        return false;
    }

    private void GenerateGoto(Closure closure)
    {
        Dictionary<Nonterminal, (ClosureItemSet Primary, LookaheadIrrelevantClosureItemSet Secondary)> itemsToAdd =
            new();
        // We have to separately maintain a list to add, because closure items can change when we
        // call AddKernelItem
        foreach (var item in closure.Kernels.Concat(closure.NonKernels))
        {
            if (item.AfterDot is null)
            {
                FinishedItems.Add((closure.Id, item));
                continue;
            }

            var newItem = new ClosureItem(item.Rule, item.Production, item.Index + 1, item.Lookahead);
            itemsToAdd.TryAdd(item.AfterDot, (new ClosureItemSet(), new LookaheadIrrelevantClosureItemSet()));

            itemsToAdd[item.AfterDot].Primary.Add(newItem);
            itemsToAdd[item.AfterDot].Secondary.Add(new ClosureItem(newItem.Rule, newItem.Production, newItem.Index));
        }

        foreach (var (nonterminal, (primeItems, secondaryItems)) in itemsToAdd)
        {
            AddKernelItem(primeItems, secondaryItems, out var newClosure, out var closureNew);
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