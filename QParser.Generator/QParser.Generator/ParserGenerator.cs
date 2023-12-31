﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        // Console.WriteLine(metaGrammar);
        var metaGrammarParser = new CanonicalLRParser(metaGrammar, metaGrammarFileInformation);
        metaGrammarParser.Generate();
        // metaGrammarParser.DumpTables();
        var enums = context.SyntaxProvider.CreateSyntaxProvider(static (n, _) => n is EnumDeclarationSyntax,
                static (n, _) =>
                {
                    // Load every enum declaration 
                    var enumDeclarationSymbol = (ITypeSymbol)n.SemanticModel.GetDeclaredSymbol(n.Node);
                    return (enumDeclarationSymbol.Name, enumDeclarationSymbol.GetEnumsFromSyntax());
                }).Collect()
            .Select(static (x, _) => x.ToDictionary(tuple => tuple.Name, tuple => tuple.Item2));
        var grammarFiles = context.AdditionalTextsProvider.Where(static f => f.Path.EndsWith(".qg"));
        var output = grammarFiles.Combine(enums).Select((tuple, token) =>
        {
            var (additionalText, enums) = tuple;
            return GenerateParseTreeFor(additionalText, token, metaGrammarParser, enums);
        });
        context.RegisterSourceOutput(output, (productionContext, tuple) =>
        {
            var (name, parseTree, fileInformation, grammar) = tuple;
            if (parseTree == null)
                foreach (var exception in fileInformation.Exceptions)
                    productionContext.ReportDiagnostic(
                        Diagnostic.Create(
                            new DiagnosticDescriptor("QE001", "Syntax Error", exception.ToString(), "Metagrammar",
                                DiagnosticSeverity.Warning, true),
                            Location.Create(fileInformation.FilePath,
                                new TextSpan(exception.SourceRange.Start.Column, 1),
                                exception.SourceRange.ToLinePositionSpan())));

            productionContext.AddSource($"{name}.g.cs",
                $$""""
                  namespace QParser.Generator.Sample;
                  // <auto-generated/>
                  public class A{{name}} : TestGrammar {
                        public override string Get() => """
                  {{parseTree}}
                  """;
                        public static string Grammar = """
                  {{grammar}}
                  """;
                  };
                  """");
        });
    }

    private static (string Name, ParseTreeNode? Tree, FileInformation fileInformation, Grammar? grammar)
        GenerateParseTreeFor(AdditionalText grammarFile, CancellationToken cancellationToken,
            Parser.QParser metaGrammarParser, Dictionary<string, Dictionary<string, int>> enumDictionary)
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
            return (fileName, null, fileInfo, null);

        var parseTree = metaGrammarParser.GetParseTree();
        Console.WriteLine(parseTree);
        metaGrammarParser.Reset();
        var grammarContext = new GrammarContext(fileInfo, enumDictionary);
        grammarContext.Traverse(parseTree);
        grammarContext.FileInformation.DumpExceptions();
        grammarContext.GrammarConstructor.Grammar.GenerateAll();
        Console.WriteLine("Complete");
        return (fileName, parseTree, fileInfo, grammarContext.GrammarConstructor.Grammar);
    }
}