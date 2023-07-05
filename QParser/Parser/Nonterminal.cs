using System.Text;
using QParser.Lexer;

namespace QParser.Parser;

public abstract class Nonterminal
{
    public string Name = "?";
    public readonly Grammar Grammar;
    public readonly HashSet<TokenType> First = new();
    public readonly HashSet<TokenType> Follow = new();
    public readonly HashSet<Nonterminal> FirstGenerator = new();
    protected bool FirstGeneratorGenerated;
    public bool CanBeEmpty;
    public bool CanBeEmptyGenerated;

    protected internal abstract void InternalGenerateFirstGenerator();

    public void GenerateFirstGenerator()
    {
        if (FirstGeneratorGenerated) return;
        InternalGenerateFirstGenerator();
        FirstGeneratorGenerated = true;
    }

    public bool RoundGenerateFirst()
    {
        var changed = false;
        foreach (var nonterminal in FirstGenerator)
        {
            changed |= First.UnionAndCheckChange(nonterminal.First);
        }

        return changed;
    }

    protected abstract void InternalGenerateCanBeEmpty();

    public bool GenerateCanBeEmpty()
    {
        if (CanBeEmptyGenerated) return false;
        var prev = CanBeEmpty;
        InternalGenerateCanBeEmpty();
        return prev != CanBeEmpty;
    }

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