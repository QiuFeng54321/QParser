using System;
using System.Collections.Generic;
using System.Linq;
using QParser.Lexer;
using QParser.Parser;

namespace QParser.Generator;

public class GrammarContext
{
    public readonly FileInformation FileInformation;
    public readonly PublicGrammarConstructor GrammarConstructor = new();
    public readonly Dictionary<string, int> Tokens = new();

    public GrammarContext(FileInformation fileInformation)
    {
        FileInformation = fileInformation;
    }

    public void FillTokens(Token typeNameToken)
    {
        var typeName = typeNameToken.Content.StringExprToString();
        var type = typeName.FindType();
        if (type is null)
        {
            new PrettyException(FileInformation, typeNameToken.SourceRange, $"Unknown type: {typeName}")
                .AddToExceptions();
            return;
        }

        if (!type.IsEnum && type.GetEnumUnderlyingType() != typeof(int))
        {
            new PrettyException(FileInformation, typeNameToken.SourceRange, $"{typeName} is not an integer enum type")
                .AddToExceptions();
            return;
        }

        foreach (var tokenName in type.GetEnumNames())
        {
            var tokenValue = (int)Enum.Parse(type, tokenName);
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
                        parseTreeNode.Data = GrammarConstructor.R(ruleNameToken.Content);
                        break;
                    }
                    case "Token":
                    {
                        var tokenNameToken = ((TokenParseTreeNode)ruleParseTreeNode.Nodes[0]).Token;
                        if (!Tokens.TryGetValue(tokenNameToken.Content, out var tokenType))
                            throw new NotImplementedException();

                        parseTreeNode.Data = GrammarConstructor.T(tokenType);
                        break;
                    }
                    case "Statement":
                    {
                        switch (ruleParseTreeNode.Production.Components[0])
                        {
                            case TokenTerminal { TokenType: (int)DefaultTokenType.QuestionMark }:
                            {
                                var macroKey = ((TokenParseTreeNode)ruleParseTreeNode.Nodes[1]).Token;
                                var content = ((TokenParseTreeNode)ruleParseTreeNode.Nodes[3]).Token;
                                if (macroKey.Content == "Tokens") FillTokens(content);

                                break;
                            }
                            case Rule { Name: "Rule" }:
                            {
                                var macroKey = ruleParseTreeNode.Nodes[0].Data as Rule ??
                                               throw new InvalidOperationException();
                                var content = ruleParseTreeNode.Nodes[2].Data as HashSet<CompositeNonterminal> ??
                                              throw new InvalidOperationException();
                                parseTreeNode.Data = GrammarConstructor.R(macroKey.Name, false, content);
                                break;
                            }
                            case TokenTerminal { TokenType: (int)DefaultTokenType.Identifier }:
                            {
                                var tokenName = ((TokenParseTreeNode)ruleParseTreeNode.Nodes[0]).Token;
                                var id = ((TokenParseTreeNode)ruleParseTreeNode.Nodes[2]).Token;
                                Tokens.TryAdd(tokenName.Content, int.Parse(id.Content));
                                break;
                            }
                        }

                        break;
                    }
                    case "Symbol":
                    {
                        var core = (Nonterminal)(parseTreeNode.Nodes[0].Data ?? throw new InvalidOperationException());
                        if (ruleParseTreeNode.Nodes.Count == 1)
                        {
                            parseTreeNode.Data = core;
                            break;
                        }

                        switch (((TokenParseTreeNode)ruleParseTreeNode.Nodes[1]).TokenType)
                        {
                            case (int)DefaultTokenType.Plus:
                            {
                                parseTreeNode.Data =
                                    GrammarConstructor.OneOrMany(GrammarConstructor.TempRuleName, core);
                                break;
                            }
                            case (int)DefaultTokenType.Multiply:
                            {
                                parseTreeNode.Data =
                                    GrammarConstructor.ZeroOrMore(GrammarConstructor.TempRuleName, core);
                                break;
                            }
                            case (int)DefaultTokenType.QuestionMark:
                            {
                                parseTreeNode.Data =
                                    GrammarConstructor.Optional(GrammarConstructor.TempRuleName, core);
                                break;
                            }
                        }

                        break;
                    }
                    case "Composite":
                    {
                        parseTreeNode.Data =
                            GrammarConstructor.C(parseTreeNode.Nodes.Select(n => (Nonterminal)n.Data).ToArray());
                        break;
                    }
                    case "Composites":
                    {
                        HashSet<CompositeNonterminal> composites = new()
                        {
                            parseTreeNode.Nodes[0].Data as CompositeNonterminal ?? throw new InvalidOperationException()
                        };
                        foreach (var trailingNode in parseTreeNode.Nodes[1].Nodes)
                            composites.Add(trailingNode.Nodes[1].Data as CompositeNonterminal ??
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