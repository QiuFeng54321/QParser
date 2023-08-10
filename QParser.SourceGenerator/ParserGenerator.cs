using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using QParser.Lexer;
using QParser.Parser;
using QParser.Parser.LR;

namespace QParser.SourceGenerator;

[Generator]
public class ParserGenerator : ISourceGenerator
{
    private readonly FileInformation _grammarFileInformation = new("Internal");
    private Grammar _grammar = null!;
    private CanonicalLRParser _grammarParser = null!;

    public void Initialize(GeneratorInitializationContext context)
    {
        var constructor = new ParserGrammarConstructor();
        constructor.Construct();
        _grammar = constructor.Grammar;
        _grammarParser = new CanonicalLRParser(_grammar, _grammarFileInformation);
        _grammarParser.Generate();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        foreach (var grammarFile in context.AdditionalFiles.Where(f => f.Path.EndsWith(".qg")))
        {
            var fileName = Path.GetFileNameWithoutExtension(grammarFile.Path);
            var fileInfo = new FileInformation(grammarFile.Path);
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(grammarFile.GetText()!.ToString()));
            var lexer = new QLexer(new SourceInputStream(fileInfo, textStream), fileInfo);
            foreach (var token in lexer)
            {
                if (token.Ignore ||
                    (DefaultTokenType)token.TokenType is DefaultTokenType.Indent or DefaultTokenType.Dedent) continue;
                _grammarParser.Feed(token);
            }

            if (!_grammarParser.Accepted)
            {
                fileInfo.DumpExceptions();
                continue;
            }

            var parseTree = _grammarParser.GetParseTree();
            Console.WriteLine(parseTree);
            context.AddSource(fileName, $"class {fileName} {{ string s = @\"{parseTree}\";}};");
        }

        context.AddSource("A", SourceText.From("class a { string s = @\"\";};", Encoding.UTF8));
    }
}