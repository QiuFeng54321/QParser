namespace QParser.Generator.ParserAst;

public class ParserZeroOrMore : ParserSymbol
{
    public ParserZeroOrMore(GrammarContext grammarContext, ParserSymbol repeatingUnit) : base(grammarContext,
        grammarContext.GrammarConstructor.ZeroOrMore(
            grammarContext.GrammarConstructor.TempRuleName, repeatingUnit.Nonterminal))
    {
        RepeatingUnit = repeatingUnit;
    }

    public ParserSymbol RepeatingUnit { get; set; }
}