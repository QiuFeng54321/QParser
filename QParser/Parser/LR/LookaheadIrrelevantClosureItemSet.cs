namespace QParser.Parser.LR;

public class LookaheadIrrelevantClosureItemSet : HashSet<ClosureItem>
{
    public LookaheadIrrelevantClosureItemSet() : base(ClosureItem.LookaheadIrrelevantComparer)
    {
    }
}