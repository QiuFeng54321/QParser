using QParser.Lexer;

namespace QParser.Parser;

public class LRParser
{
    private readonly Dictionary<(int state, TokenType tokenType), LRAction> _actionTable = new();
    private readonly LR1ClosureTable _closureTable;
    private readonly Dictionary<(int state, Rule rule), LRAction> _gotoTable = new();

    public LRParser(Grammar grammar)
    {
        grammar.GenerateAll();
        _closureTable = new LR1ClosureTable(grammar.EntryRule ?? throw new InvalidOperationException());
    }

    public bool GenerateTables()
    {
        _closureTable.Generate();
        var isLR1 = true;
        foreach (var ((id, symbol), closure) in _closureTable.GotoTable)
            if (symbol is TokenTerminal tokenTerminal)
            {
                if (tokenTerminal.TokenType is TokenType.Eof)
                    _actionTable[(id, TokenType.Eof)] = new LRAction();
                else if (!_actionTable.TryAdd((id, tokenTerminal.TokenType), new LRAction(closure.Id))) isLR1 = false;
            }
            else if (symbol is Rule rule)
            {
                if (!_gotoTable.TryAdd((id, rule), new LRAction(closure.Id))) isLR1 = false;
            }

        foreach (var (id, item) in _closureTable.FinishedItems)
            if (!_actionTable.TryAdd((id, item.Lookahead), new LRAction(item.Rule, item.Production)))
                isLR1 = false;

        return isLR1;
    }

    public void Dump()
    {
        foreach (var ((state, tokenType), action) in _actionTable)
            Console.WriteLine($"Action {state}, {tokenType} -> {action}");
        foreach (var ((state, rule), action) in _gotoTable) Console.WriteLine($"Goto {state}, {rule} -> {action}");
    }
}