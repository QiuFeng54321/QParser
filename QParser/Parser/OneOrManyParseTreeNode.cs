using System.Collections.Generic;
using System.Linq;

namespace QParser.Parser;

public class OneOrManyParseTreeNode : RuleParseTreeNode
{
    public OneOrManyParseTreeNode(Rule rule, CompositeNonterminal production, List<ParseTreeNode> nodes) : base(rule,
        production,
        nodes.Count > 1 ? nodes[0].Nodes.Append(nodes[1]).ToList() : nodes)
    {
    }
}