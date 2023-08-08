using QParser.Lexer;
using QParser.Lexer.Tokens;

namespace QParser.Parser;

public class SLRParser : QParser
{
    private readonly Dictionary<(int state, int tokenType), LRAction> _actionTable = new();
    private readonly LR0ClosureTable _closureTable;
    private readonly Dictionary<(int state, Rule rule), LRAction> _gotoTable = new();

    public SLRParser(Grammar grammar, FileInformation fileInformation) : base(grammar, fileInformation)
    {
        grammar.GenerateAll();
        _closureTable = new LR0ClosureTable(grammar.EntryRule ?? throw new InvalidOperationException());
    }

    public override bool Accepted { get; }
    public override bool IsParserValid { get; protected set; }

    public bool GenerateTables()
    {
        _closureTable.Generate();
        var isSLR1 = true;
        foreach (var ((id, symbol), closure) in _closureTable.GotoTable)
            if (symbol is TokenTerminal tokenTerminal)
            {
                if (tokenTerminal.TokenType is TokenConstants.Eof)
                    _actionTable[(id, TokenConstants.Eof)] = new LRAction();
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

    public override void Generate()
    {
        IsParserValid = GenerateTables();
    }

    public override void Feed(Token token)
    {
        throw new NotImplementedException();
    }

    public override void DumpStates()
    {
        throw new NotImplementedException();
    }

    public override void DumpTables()
    {
        foreach (var ((state, tokenType), action) in _actionTable)
            Console.WriteLine($"Action {state}, {tokenType} -> {action}");
        foreach (var ((state, rule), action) in _gotoTable) Console.WriteLine($"Goto {state}, {rule} -> {action}");
    }
}