using QParser.Parser;

namespace QParser.Generator;

public class GrammarContext
{
    public readonly PublicGrammarConstructor GrammarConstructor = new();

    public void Traverse(ParseTreeNode parseTreeNode)
    {
        foreach (var child in parseTreeNode.Nodes) Traverse(child);

        if (parseTreeNode is RuleParseTreeNode ruleParseTreeNode)
            switch (ruleParseTreeNode.Rule.Name)
            {
                case "Rule":
                {
                    var ruleNameToken = ((TokenParseTreeNode)ruleParseTreeNode.Nodes[1]).Token;
                    GrammarConstructor.R(ruleNameToken.Content);
                    break;
                }
            }
    }
}