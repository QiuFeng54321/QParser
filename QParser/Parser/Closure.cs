using System.Text;

namespace QParser.Parser;

public class Closure
{
    public readonly int Id;
    public readonly HashSet<ClosureItem> Kernels;
    public readonly HashSet<ClosureItem> NonKernels = new();

    /// <summary>
    ///     Initializes the closure with an id and an initial item.
    ///     The item will be expanded.
    /// </summary>
    /// <param name="id">The unique id assigned to this closure.</param>
    /// <param name="kernels">The kernel items</param>
    public Closure(int id, HashSet<ClosureItem> kernels)
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
        NonKernels.Add(item);
        if (item.AfterDot is { } afterDot) NonKernels.UnionWith(afterDot.NonKernelItems);
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