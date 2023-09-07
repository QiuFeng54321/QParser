namespace QParser.Generator.ParserAst;

public class ParserOptional : ParserSymbol
{
    public ParserOptional(GrammarContext grammarContext, ParserSymbol repeatingUnit) : base(grammarContext,
        grammarContext.GrammarConstructor.Optional(
            grammarContext.GrammarConstructor.TempRuleName, repeatingUnit.Nonterminal))
    {
        RepeatingUnit = repeatingUnit;
    }

    public ParserSymbol RepeatingUnit { get; set; }
}