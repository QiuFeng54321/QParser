namespace QParser.Parser.LR;

public class ClosureItemSet : HashSet<ClosureItem>
{
    // public ClosureItemSet() : base(ClosureItem.LookaheadIrrelevantComparer)
    // {
    // }

    // public new bool Add(ClosureItem item)
    // {
    //     if (!TryGetValue(item, out var existingItem)) return base.Add(item);
    //     if (existingItem.Lookahead is null || item.Lookahead is null) return false;
    //     if (item.Lookahead.IsSubsetOf(existingItem.Lookahead)) return false;
    //     existingItem.Lookahead.UnionWith(item.Lookahead);
    //     return true;
    // }
}