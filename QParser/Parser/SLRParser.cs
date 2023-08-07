using QParser.Lexer;

namespace QParser.Parser;

public class SLRParser
{
    private readonly Dictionary<(int state, TokenType tokenType), LRAction> _actionTable = new();
    private readonly LR0ClosureTable _closureTable;
    private readonly Dictionary<(int state, Rule rule), LRAction> _gotoTable = new();

    public SLRParser(Grammar grammar)
    {
        grammar.GenerateAll();
        _closureTable = new LR0ClosureTable(grammar.EntryRule ?? throw new InvalidOperationException());
    }

    public bool GenerateTables()
    {
        _closureTable.Generate();
        var isSLR1 = true;
        foreach (var ((id, symbol), closure) in _closureTable.GotoTable)
            if (symbol is TokenTerminal tokenTerminal)
            {
                if (tokenTerminal.TokenType is TokenType.Eof)
                    _actionTable[(id, TokenType.Eof)] = new LRAction();
                else if (!_actionTable.TryAdd((id, tokenTerminal.TokenType), new LRAction(closure.Id))) isSLR1 = false;
            }
            else if (symbol is Rule rule)
            {
                if (!_gotoTable.TryAdd((id, rule), new LRAction(closure.Id))) isSLR1 = false;
            }

        foreach (var (id, (rule, production, _, _)) in _closureTable.FinishedItems)
        foreach (var followToken in rule.Follow)
            if (!_actionTable.TryAdd((id, followToken), new LRAction(rule, production)))
                isSLR1 = false;

        return isSLR1;
    }

    public void Dump()
    {
        foreach (var ((state, tokenType), action) in _actionTable)
            Console.WriteLine($"Action {state}, {tokenType} -> {action}");
        foreach (var ((state, rule), action) in _gotoTable) Console.WriteLine($"Goto {state}, {rule} -> {action}");
    }
}