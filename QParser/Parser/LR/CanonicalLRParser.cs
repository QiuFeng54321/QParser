using System;

namespace QParser.Parser.LR;

public class CanonicalLRParser : LRParser
{
    public CanonicalLRParser(Grammar grammar, FileInformation fileInformation) : base(grammar, fileInformation,
        new LR1ClosureTable(grammar.EntryRule ?? throw new InvalidOperationException()))
    {
    }

    public override bool GenerateReduce()
    {
        var isLR1 = true;
        foreach (var (id, item) in ClosureTable.FinishedItems)
            if (!ActionTable.TryAdd((id, item.Lookahead), new ReduceLRAction(item.Rule, item.Production)))
            {
                GenerationErrors.Add(new Exception($"Reduce conflict in ACTION[{id}, {item.Lookahead}]"));
                isLR1 = false;
            }

        return isLR1;
    }
}