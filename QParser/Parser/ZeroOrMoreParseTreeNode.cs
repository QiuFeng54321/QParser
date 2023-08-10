using System.Collections.Generic;

namespace QParser.Parser;

public class ZeroOrMoreParseTreeNode : RuleParseTreeNode
{
    public ZeroOrMoreParseTreeNode(Rule rule, CompositeNonterminal production, List<ParseTreeNode> nodes) : base(rule,
        production,
        nodes[0].Nodes)
    {
        if (nodes.Count > 1) Nodes.Add(nodes[1]);
    }
}