namespace QParser.Generator.ParserAst;

public class ParserStatements : ParserAstNode
{
    public ParserStatement[] Statements;

    public ParserStatements(GrammarContext grammarContext, ParserStatement[] statements) : base(grammarContext)
    {
        Statements = statements;
    }
}