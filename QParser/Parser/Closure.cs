using System.Text;

namespace QParser.Parser;

public class Closure
{
    public readonly int Id;
    public readonly HashSet<ClosureItem> Items = new();

    /// <summary>
    ///     Initializes the closure with an id and an initial item.
    ///     The item will be expanded.
    /// </summary>
    /// <param name="id">The unique id assigned to this closure.</param>
    /// <param name="item">The item to be added and expanded</param>
    public Closure(int id, ClosureItem item)
    {
        Id = id;
        Expand(item);
    }

    /// <summary>
    ///     Add the item and all of its non-kernel items to this closure
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <returns>If the item set has been changed after this call</returns>
    public bool Expand(ClosureItem item)
    {
        var prevLen = Items.Count;
        Items.Add(item);
        if (item.AfterDot is { } afterDot) Items.UnionWith(afterDot.NonKernelItems);
        return prevLen != Items.Count;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Closure {Id}:");
        foreach (var item in Items)
            sb.Append("    ")
                .AppendLine(item.ToString());

        return sb.ToString();
    }
}