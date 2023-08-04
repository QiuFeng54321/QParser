using QParser.Lexer;

namespace QParser.Parser;

public abstract class Nonterminal
{
    public readonly HashSet<TokenType> First = new();
    public readonly HashSet<Nonterminal> FirstGenerator = new();
    public readonly HashSet<TokenType> Follow = new();
    public readonly HashSet<Nonterminal> FollowGenerator = new();
    protected readonly Grammar Grammar;
    public bool CanBeEmpty;
    public bool CanBeEmptyGenerated;
    protected bool FirstGeneratorGenerated;

    public string Name = "?";
    public HashSet<ClosureItem> NonKernelItems = new();

    protected Nonterminal(Grammar grammar)
    {
        Grammar = grammar;
    }

    protected abstract void InternalGenerateFirstGenerator();

    /// <summary>
    ///     Generates CLOSURE(item), where item is this nonterminal whose dot index is 0
    /// </summary>
    /// <returns>If CLOSURE has changed</returns>
    public abstract bool GenerateNonKernelItems();

    public void GenerateFirstGenerator()
    {
        if (FirstGeneratorGenerated) return;
        InternalGenerateFirstGenerator();
        FirstGeneratorGenerated = true;
    }

    /// <summary>
    ///     Performs a round of adding FIRST(x) for x in FirstGenerator to FIRST(this) and checks
    ///     if FIRST(this) is changed
    /// </summary>
    /// <returns>If FIRST(this) has a change in length</returns>
    public bool RoundGenerateFirst()
    {
        var changed = false;
        foreach (var nonterminal in FirstGenerator) changed |= First.UnionAndCheckChange(nonterminal.First);

        return changed;
    }

    public bool RoundGenerateFollow()
    {
        var changed = false;
        foreach (var nonterminal in FollowGenerator) changed |= Follow.UnionAndCheckChange(nonterminal.Follow);

        return changed;
    }

    protected abstract void InternalGenerateCanBeEmpty();

    /// <summary>
    ///     Performs a round of updating CanBeEmpty.
    /// </summary>
    /// <returns>If CanBeEmpty has changed its value</returns>
    public bool GenerateCanBeEmpty()
    {
        if (CanBeEmptyGenerated) return false;
        var prev = CanBeEmpty;
        InternalGenerateCanBeEmpty();
        return prev != CanBeEmpty;
    }

    public override string ToString()
    {
        return Name;
    }
}