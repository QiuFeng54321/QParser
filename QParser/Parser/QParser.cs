using QParser.Lexer.Tokens;

namespace QParser.Parser;

public abstract class QParser
{
    protected readonly Grammar Grammar;
    protected FileInformation FileInformation;

    protected QParser(Grammar grammar, FileInformation fileInformation)
    {
        Grammar = grammar;
        FileInformation = fileInformation;
    }

    public abstract bool Accepted { get; }
    public abstract bool IsParserValid { get; protected set; }

    public abstract void Generate();
    public abstract void Feed(Token token);
    public abstract void DumpStates();
    public abstract void DumpTables();
    public abstract ParseTreeNode GetParseTree();
}