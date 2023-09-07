namespace QParser.Generator.ParserAst;

public class ParserOneOrMany : ParserSymbol
{
    public ParserOneOrMany(GrammarContext grammarContext, ParserSymbol repeatingUnit) : base(grammarContext,
        grammarContext.GrammarConstructor.OneOrMany(
            grammarContext.GrammarConstructor.TempRuleName, repeatingUnit.Nonterminal))
    {
        RepeatingUnit = repeatingUnit;
    }

    public ParserSymbol RepeatingUnit { get; set; }
}