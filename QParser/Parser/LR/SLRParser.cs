using System;

namespace QParser.Parser.LR;

public class SLRParser : LRParser
{
    public SLRParser(Grammar grammar, FileInformation fileInformation) : base(grammar, fileInformation,
        new LR0ClosureTable(grammar.EntryRule ?? throw new InvalidOperationException()))
    {
    }

    public override bool GenerateReduce()
    {
        var isSLR1 = true;
        foreach (var (id, (rule, production, _, _)) in ClosureTable.FinishedItems)
        foreach (var followToken in rule.Follow)
            if (!ActionTable.TryAdd((id, followToken), new ReduceLRAction(rule, production)))
            {
                GenerationErrors.Add(new Exception($"Reduce conflict in ACTION[{id}, {followToken}]"));
                isSLR1 = false;
            }

        return isSLR1;
    }
}