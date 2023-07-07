namespace QParser.Parser;

public class NonterminalWithAction : Nonterminal
{
    public NonterminalWithAction(Grammar grammar) : base(grammar)
    {
        FirstGeneratorGenerated = true;
        CanBeEmpty = true;
        CanBeEmptyGenerated = true;
        Name = "#";
    }

    protected override void InternalGenerateFirstGenerator()
    {
    }

    protected override void InternalGenerateCanBeEmpty()
    {
    }
}