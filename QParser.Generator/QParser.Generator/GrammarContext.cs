using System;
using System.Collections.Generic;
using System.Linq;
using QParser.Generator.ParserAst;
using QParser.Lexer;
using QParser.Parser;

namespace QParser.Generator;

public class GrammarContext
{
    public readonly FileInformation FileInformation;
    public readonly PublicGrammarConstructor GrammarConstructor = new();
    public readonly Dictionary<string, int> Tokens = new();

    public GrammarContext(FileInformation fileInformation, Dictionary<string, Dictionary<string, int>> enumDictionary)
    {
        FileInformation = fileInformation;
        EnumDictionary = enumDictionary;
    }

    public Dictionary<string, Dictionary<string, int>> EnumDictionary { get; set; }

    public void FillTokens(Token typeNameToken)
    {
        var typeName = typeNameToken.Content.StringExprToString();
        if (!EnumDictionary.TryGetValue(typeName, out var enumValues))
        {
            var type = typeName.FindType();
            if (type is null)
            {
                new PrettyException(FileInformation, typeNameToken.SourceRange, $"Unknown type: {typeName}")
                    .AddToExceptions();
                return;
            }

            if (!type.IsEnum || type.GetEnumUnderlyingType() != typeof(int))
            {
                new PrettyException(FileInformation, typeNameToken.SourceRange,
                        $"{typeName} is not an integer enum type")
                    .AddToExceptions();
                return;
            }

            enumValues = type.GetEnumsFromType();
            EnumDictionary.Add(typeName, enumValues);
        }

        foreach (var (tokenName, tokenValue) in enumValues)
        {
            Tokens[tokenName] = tokenValue;
        }
    }

    public void Traverse(ParseTreeNode parseTreeNode)
    {
        foreach (var child in parseTreeNode.Nodes) Traverse(child);


        switch (parseTreeNode)
        {
            case RuleParseTreeNode ruleParseTreeNode:
            {
                switch (ruleParseTreeNode.Rule.Name)
                {
                    case "Rule":
                    {
                        var ruleNameToken = ((TokenParseTreeNode)ruleParseTreeNode.Nodes[1]).Token;
                        parseTreeNode.Data = new ParserRule(this, ruleNameToken);
                        break;
                    }
                    case "Token":
                    {
                        var tokenNameToken = ((TokenParseTreeNode)ruleParseTreeNode.Nodes[0]).Token;
                        parseTreeNode.Data = new ParserToken(this, tokenNameToken);
                        break;
                    }
                    case "Statement":
                    {
                        switch (ruleParseTreeNode.Production.Components[0])
                        {
                            // Macro
                            case TokenTerminal { TokenType: (int)DefaultTokenType.QuestionMark }:
                            {
                                var macroKey = ((TokenParseTreeNode)ruleParseTreeNode.Nodes[1]).Token;
                                var content = ((TokenParseTreeNode)ruleParseTreeNode.Nodes[3]).Token;
                                parseTreeNode.Data = new ParserMacro(this, macroKey, content);
                                break;
                            }
                            case Rule { Name: "Rule" }:
                            {
                                var macroKey = ruleParseTreeNode.Nodes[0].Data as ParserRule ??
                                               throw new InvalidOperationException();
                                var content = ruleParseTreeNode.Nodes[2].Data as List<ParserCompositeRule> ??
                                              throw new InvalidOperationException();
                                parseTreeNode.Data = new ParserRuleDeclaration(this, content, macroKey);
                                break;
                            }
                            case TokenTerminal { TokenType: (int)DefaultTokenType.Identifier }:
                            {
                                var tokenName = ((TokenParseTreeNode)ruleParseTreeNode.Nodes[0]).Token;
                                var id = ((TokenParseTreeNode)ruleParseTreeNode.Nodes[2]).Token;
                                parseTreeNode.Data = new ParserTokenDeclaration(this, tokenName, id);
                                break;
                            }
                        }

                        break;
                    }
                    case "Symbol":
                    {
                        var core = (ParserSymbol)(parseTreeNode.Nodes[0].Data ?? throw new InvalidOperationException());
                        if (ruleParseTreeNode.Nodes.Count == 1)
                        {
                            parseTreeNode.Data = core;
                            break;
                        }

                        switch (((TokenParseTreeNode)ruleParseTreeNode.Nodes[1]).TokenType)
                        {
                            case (int)DefaultTokenType.Plus:
                            {
                                parseTreeNode.Data = new ParserOneOrMany(this, core);
                                break;
                            }
                            case (int)DefaultTokenType.Multiply:
                            {
                                parseTreeNode.Data = new ParserZeroOrMore(this, core);
                                break;
                            }
                            case (int)DefaultTokenType.QuestionMark:
                            {
                                parseTreeNode.Data = new ParserOptional(this, core);
                                break;
                            }
                        }

                        break;
                    }
                    case "Composite":
                    {
                        parseTreeNode.Data = new ParserCompositeRule(this,
                            parseTreeNode.Nodes.Select(n => (ParserSymbol)n.Data).ToList());
                        break;
                    }
                    case "Composites":
                    {
                        List<ParserCompositeRule> composites = new()
                        {
                            parseTreeNode.Nodes[0].Data as ParserCompositeRule ?? throw new InvalidOperationException()
                        };
                        foreach (var trailingNode in parseTreeNode.Nodes[1].Nodes)
                            composites.Add(trailingNode.Nodes[1].Data as ParserCompositeRule ??
                                           throw new InvalidOperationException());

                        parseTreeNode.Data = composites;
                        break;
                    }
                }

                break;
            }
        }
    }
}