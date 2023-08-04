namespace QParser.Parser;

public abstract class RoundGenerator<TElement>
{
    public readonly HashSet<TElement> Elements = new();
    public readonly HashSet<RoundGenerator<TElement>> Generators = new();
    public bool ElementsGenerated;
    public bool GeneratorGenerated;
    protected abstract void InternalGenerateFirstGenerator();

    public void GenerateGenerator()
    {
        if (GeneratorGenerated) return;
        InternalGenerateFirstGenerator();
        GeneratorGenerated = true;
    }

    public bool RoundGenerateFirst()
    {
        if (ElementsGenerated) return false;
        var changed = false;
        foreach (var nonterminal in Generators) changed |= Elements.UnionAndCheckChange(nonterminal.Elements);
        return changed;
    }
}