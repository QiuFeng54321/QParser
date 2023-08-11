﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using QParser.Lexer;
using QParser.Parser;
using QParser.Parser.LR;

namespace QParser.Generator;

[Generator]
public class ParserGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var metaGrammarFileInformation = new FileInformation("Internal");
        var metaGrammarConstructor = new ParserGrammarConstructor();
        metaGrammarConstructor.Construct();
        var metaGrammar = metaGrammarConstructor.Grammar;
        metaGrammar.GenerateAll();
        Console.WriteLine(metaGrammar);
        var metaGrammarParser = new CanonicalLRParser(metaGrammar, metaGrammarFileInformation);
        metaGrammarParser.Generate();
        metaGrammarParser.DumpTables();
        var grammarFiles = context.AdditionalTextsProvider.Where(static f => f.Path.EndsWith(".qg"));
        var output = grammarFiles.Select((additionalText, token) =>
            GenerateParseTreeFor(additionalText, token, metaGrammarParser));

        context.RegisterSourceOutput(output, (productionContext, tuple) =>
        {
            var (name, parseTree, fileInformation) = tuple;
            if (parseTree == null)
                foreach (var exception in fileInformation.Exceptions)
                    productionContext.ReportDiagnostic(
                        Diagnostic.Create(
                            new DiagnosticDescriptor("QE002", "Syntax Error", exception.ToString(), "Metagrammar",
                                DiagnosticSeverity.Info, true),
                            Location.Create(fileInformation.FilePath,
                                new TextSpan(exception.SourceRange.Start.Column, 1),
                                exception.SourceRange.ToLinePositionSpan())));

            productionContext.AddSource($"{name}.g.cs",
                $$"""
                  // <auto-generated/>
                  namespace QParser.Generator.Sample;
                  public class A{{name}} : TestGrammar {
                        public override string Get() => @"{{parseTree}}";
                        };
                  """);
        });
    }

    public void Execute(GeneratorExecutionContext context)
    {
    }

    private static (string Name, ParseTreeNode? Tree, FileInformation fileInformation) GenerateParseTreeFor(
        AdditionalText grammarFile, CancellationToken cancellationToken, Parser.QParser metaGrammarParser)
    {
        var fileName = Path.GetFileNameWithoutExtension(grammarFile.Path);
        var fileInfo = new FileInformation(grammarFile.Path);
        metaGrammarParser.FileInformation = fileInfo;
        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(grammarFile.GetText(cancellationToken)!.ToString()));
        var lexer = new QLexer(new SourceInputStream(fileInfo, textStream), fileInfo);
        foreach (var token in lexer)
        {
            if (token.Ignore ||
                token.TokenType == (int)DefaultTokenType.Indent ||
                token.TokenType == (int)DefaultTokenType.Dedent) continue;
            Console.WriteLine(token);
            metaGrammarParser.Feed(token);
            metaGrammarParser.DumpStates();
        }

        fileInfo.DumpExceptions();
        if (!metaGrammarParser.Accepted || fileInfo.Exceptions.Count > 0)
            return (fileName, null, fileInfo);

        var parseTree = metaGrammarParser.GetParseTree();
        Console.WriteLine(parseTree);
        metaGrammarParser.Reset();
        return (fileName, parseTree, fileInfo);
    }
}