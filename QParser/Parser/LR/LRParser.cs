using System.Diagnostics;
using QParser.Lexer;
using QParser.Lexer.Tokens;

namespace QParser.Parser.LR;

public abstract class LRParser : QParser
{
    private readonly Stack<int> _stateStack = new();
    private readonly Stack<ParseTreeNode> _treeNodeStack = new();
    protected readonly Dictionary<(int state, int tokenType), LRAction> ActionTable = new();
    protected readonly ClosureTable ClosureTable;
    protected readonly Dictionary<(int state, Rule rule), LRAction> GotoTable = new();
    private bool _accepted;

    public LRParser(Grammar grammar, FileInformation fileInformation, ClosureTable closureTable) : base(grammar,
        fileInformation)
    {
        ClosureTable = closureTable;
        grammar.GenerateAll();
        _stateStack.Push(0);
    }

    public override bool Accepted => _accepted;
    public override bool IsParserValid { get; protected set; }

    public abstract bool GenerateReduce();

    public bool GenerateTables()
    {
        ClosureTable.Generate();
        var isLR1 = true;
        foreach (var ((id, symbol), closure) in ClosureTable.GotoTable)
            if (symbol is TokenTerminal tokenTerminal)
            {
                if (tokenTerminal.TokenType is TokenConstants.Eof)
                    ActionTable[(id, TokenConstants.Eof)] = new AcceptLRAction();
                else if (!ActionTable.TryAdd((id, tokenTerminal.TokenType), new ShiftLRAction(closure.Id)))
                {
                    GenerationErrors.Add(new Exception($"Conflict in ACTION[{id}, {tokenTerminal.TokenType}]"));
                    isLR1 = false;
                }
            }
            else if (symbol is Rule rule)
            {
                if (!GotoTable.TryAdd((id, rule), new ShiftLRAction(closure.Id))) isLR1 = false;
            }

        isLR1 &= GenerateReduce();

        return isLR1;
    }

    public override void Generate()
    {
        IsParserValid = GenerateTables();
    }

    public override void Feed(Token token)
    {
        while (true)
        {
            var curState = _stateStack.Peek();
            var isEpsilon = false;
            if (!ActionTable.TryGetValue((curState, token.TokenType), out var action))
            {
                isEpsilon = ActionTable.TryGetValue((curState, TokenConstants.Epsilon), out action);
                if (!isEpsilon)
                {
                    new PrettyException(FileInformation, token.SourceRange,
                            $"Unexpected token: {token}")
                        .AddToExceptions();
                    break;
                }
            }

            Debug.Assert(action != null, nameof(action) + " != null");
            if (action is ShiftLRAction shiftLRAction)
            {
                var tokenType = isEpsilon ? TokenConstants.Epsilon : token.TokenType;
                var tokenNode = new TokenParseTreeNode(tokenType, token);
                _treeNodeStack.Push(tokenNode);
                _stateStack.Push(shiftLRAction.ShiftState);
                if (!isEpsilon) break;
            }
            else if (action is ReduceLRAction reduceLRAction)
            {
                var count = reduceLRAction.Production.Components.Length;
                var children = new List<ParseTreeNode>();
                for (var i = 0; i < count; i++)
                {
                    children.Insert(0, _treeNodeStack.Pop());
                    _stateStack.Pop();
                }

                var node = reduceLRAction.Rule.MakeNode(reduceLRAction.Production, children);

                curState = _stateStack.Peek();
                var gotoState = GotoTable[(curState, reduceLRAction.Rule)];
                if (gotoState is not ShiftLRAction gotoAction)
                {
                    new PrettyException(FileInformation, node.SourceRange,
                            $"Can't figure out which state to go to for ({curState}, {reduceLRAction.Rule})")
                        .AddToExceptions();
                    break;
                }

                _stateStack.Push(gotoAction.ShiftState);
                _treeNodeStack.Push(node);
            }
            else if (action is AcceptLRAction)
            {
                _accepted = true;
                break;
            }
        }
    }

    public override void DumpStates()
    {
        Console.WriteLine($"States stack: {{{string.Join(", ", _stateStack)}}}");
    }

    public override void DumpTables()
    {
        foreach (var ((state, tokenType), action) in ActionTable)
            Console.WriteLine($"Action {state}, {tokenType} -> {action}");
        foreach (var ((state, rule), action) in GotoTable) Console.WriteLine($"Goto {state}, {rule} -> {action}");
    }

    public override ParseTreeNode GetParseTree()
    {
        return _treeNodeStack.Peek();
    }
}