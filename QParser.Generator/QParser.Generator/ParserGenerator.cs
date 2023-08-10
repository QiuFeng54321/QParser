using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using QParser.Lexer;
using QParser.Parser.LR;

namespace QParser.Generator;

[Generator]
public class ParserGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // var constructor = new ParserGrammarConstructor();
        // constructor.Construct();
        // _grammar = constructor.Grammar;
        // _grammarParser = new CanonicalLRParser(_grammar, _grammarFileInformation);
        // _grammarParser.Generate();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("a", "public class a { public static string s = @\"hihi\";};");

        // var token = new Token(3, "DOSD");
        var grammarFileInformation = new FileInformation("Internal");
        var grammarConstructor = new ParserGrammarConstructor();
        grammarConstructor.Construct();
        var grammar = grammarConstructor.Grammar;
        grammar.GenerateAll();
        Console.WriteLine(grammar);
        var grammarParser = new CanonicalLRParser(grammar, grammarFileInformation);
        grammarParser.Generate();
        grammarParser.DumpTables();
        foreach (var grammarFile in context.AdditionalFiles.Where(f => f.Path.EndsWith(".qg")))
        {
            var fileName = Path.GetFileNameWithoutExtension(grammarFile.Path);
            // context.AddSource(fileName, $"public class {fileName} {{public static string b = \"{token}\";}}");
            var fileInfo = new FileInformation(grammarFile.Path);
            grammarParser.FileInformation = fileInfo;
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(grammarFile.GetText().ToString()));
            var lexer = new QLexer(new SourceInputStream(fileInfo, textStream), fileInfo);
            foreach (var token in lexer)
            {
                if (token.Ignore ||
                    token.TokenType == (int)DefaultTokenType.Indent ||
                    token.TokenType == (int)DefaultTokenType.Dedent) continue;
                Console.WriteLine(token);
                grammarParser.Feed(token);
                grammarParser.DumpStates();
            }

            fileInfo.DumpExceptions();
            if (!grammarParser.Accepted || fileInfo.Exceptions.Count > 0) continue;

            var parseTree = grammarParser.GetParseTree();
            Console.WriteLine(parseTree);
            context.AddSource(fileName,
                $"public class {fileName} {{ public static readonly string s = @\"{parseTree}\";}};");
            grammarParser.Reset();
        }
    }
}