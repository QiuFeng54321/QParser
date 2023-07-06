using System.Text;

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

    protected internal override void InternalGenerateFirstGenerator()
    {
        
    }

    protected override void InternalGenerateCanBeEmpty()
    {
    }
}