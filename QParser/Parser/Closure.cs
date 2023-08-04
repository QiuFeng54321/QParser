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

    public void Expand(ClosureItem item)
    {
        Items.Add(item);
        if (item.AfterDot is { } afterDot) Items.UnionWith(afterDot.NonKernelItems);
    }
}