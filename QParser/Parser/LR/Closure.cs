using System.Text;

namespace QParser.Parser.LR;

public class Closure
{
    public readonly int Id;
    public readonly ClosureItemSet Kernels;
    public readonly ClosureItemSet NonKernels = new();

    /// <summary>
    ///     Initializes the closure with an id and an initial item.
    ///     The item will be expanded.
    /// </summary>
    /// <param name="id">The unique id assigned to this closure.</param>
    /// <param name="kernels">The kernel items</param>
    public Closure(int id, ClosureItemSet kernels)
    {
        Id = id;
        Kernels = kernels;
        foreach (var kernel in Kernels) Expand(kernel);
        NonKernels.ExceptWith(Kernels);
    }


    /// <summary>
    ///     Add the item and all of its non-kernel items to this closure
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <returns>If the item set has been changed after this call</returns>
    private void Expand(ClosureItem item)
    {
        if (item.AfterDot is not { } afterDot) return;
        if (item.IsLR1)
            ExpandLR1(item);
        else NonKernels.UnionWith(afterDot.NonKernelItems);
    }

    private void ExpandLR1(ClosureItem item)
    {
        if (item.AfterDot is not Rule rule) return;
        ClosureItemSet addedItems = new();
        var firstAfterAfterDot = item.FirstAfterAfterDot();
        foreach (var subRule in rule.SubRules)
        foreach (var first in firstAfterAfterDot)
        {
            var newItem = new ClosureItem(rule, subRule, 0, first);
            if (NonKernels.Add(newItem)) addedItems.Add(newItem);
        }

        foreach (var addedItem in addedItems) ExpandLR1(addedItem);
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Closure {Id}:");
        sb.AppendLine("Kernels:");
        foreach (var kernel in Kernels)
            sb.Append("    ")
                .AppendLine(kernel.ToString());
        sb.AppendLine("Non-kernels:");
        foreach (var item in NonKernels)
            sb.Append("    ")
                .AppendLine(item.ToString());

        return sb.ToString();
    }
}