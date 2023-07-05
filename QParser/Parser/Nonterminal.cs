using System.Text;
using QParser.Lexer;

namespace QParser.Parser;

public abstract class Nonterminal
{
    public string Name = "?";
    public readonly Grammar Grammar;
    public readonly HashSet<TokenType> First = new();
    public readonly HashSet<TokenType> Follow = new();
    private bool _firstGenerated;
    private bool _firstGenerating;
    protected bool FollowGenerated;

    protected abstract void InternalGenerateFirst();

    public void GenerateFirst()
    {
        if (_firstGenerated) return;
        if (_firstGenerating) throw new FormatException($"Left recursive rule: {this}");
        _firstGenerating = true;
        InternalGenerateFirst();
        _firstGenerated = true;
        _firstGenerating = false;
    }
    
    public abstract bool CanBeEmpty { get; }

    protected Nonterminal(Grammar grammar)
    {
        Grammar = grammar;
    }

    public static NonterminalWithAction operator *(Nonterminal nonterminal, Action<Nonterminal> action)
    {
        return new NonterminalWithAction(nonterminal, action);
    }

    public override string ToString() => Name;
}