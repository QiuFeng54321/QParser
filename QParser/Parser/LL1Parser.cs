using System.Diagnostics;
using QParser.Lexer;
using QParser.Lexer.Tokens;

namespace QParser.Parser;

public class LL1Parser
{
    private readonly Grammar _grammar;
    private readonly List<Token> _matched = new();
    private readonly LL1ParseTable _parseTable = new();
    private readonly Stack<Nonterminal> _stack = new();

    public LL1Parser(Grammar grammar)
    {
        _grammar = grammar;
        _stack.Push(_grammar.Eof.SingleToken);
        _stack.Push(_grammar.EntryRule ?? throw new InvalidOperationException("Undefined entry rule for grammar"));
        var genRes = _parseTable.GenerateFromGrammar(_grammar);
        genRes.HasError(err => throw new FormatException(err.ToString()));
    }

    public bool Accepted => _stack.Count == 0;

    /// <summary>
    /// </summary>
    /// <param name="lookahead">Lookahead token</param>
    /// <returns>If the lookahead is consumed</returns>
    private bool Process(Token lookahead)
    {
        var next = _stack.Pop();
        if (next is TokenTerminal tokenTerminal)
        {
            if (tokenTerminal.TokenType is TokenType.Epsilon) return false;
            if (tokenTerminal.TokenType != lookahead.TokenType)
                throw new FormatException($"Unexpected token: {lookahead}");

            _matched.Add(lookahead);
            return true;
        }

        if (next is Rule rule)
        {
            if (!_parseTable.TryGet(rule, lookahead.TokenType, out var compositeNonterminal))
                throw new FormatException(
                    $"Rule {rule} expecting one of {{{string.Join(", ", rule.First)}}}, but got {lookahead}");


            Debug.Assert(compositeNonterminal != null, nameof(compositeNonterminal) + " != null");
            foreach (var nonterminal in compositeNonterminal.Components.Reverse()) _stack.Push(nonterminal);

            return false;
        }

        throw new InvalidOperationException($"Unexpected top of stack: {next}, current lookahead {lookahead}");
    }

    public void Feed(Token token)
    {
        while (!Process(token)) DumpStates();
    }

    public void DumpStates()
    {
        Console.WriteLine($"Matched: {{{string.Join(", ", _matched)}}}");
        Console.WriteLine($"Stack: {{{string.Join(", ", _stack)}}}");
    }

    public void DumpTable()
    {
        foreach (var (rule, token, compositeRule) in _parseTable)
            Console.WriteLine($"M[{rule}, {token}] = {compositeRule}");
    }
}