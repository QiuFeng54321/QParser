using System.Text;

namespace QParser.Parser;

public class Closure
{
    public readonly HashSet<ClosureItem> Items = new();
    public int Id;

    public Closure(int id, HashSet<ClosureItem> items)
    {
        Id = id;
        Items = items;
    }

    public Closure(int id, ClosureItem item)
    {
        Id = id;
        Expand(item);
    }

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