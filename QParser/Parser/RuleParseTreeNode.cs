using System.Collections.Generic;
using System.Text;

namespace QParser.Parser;

public class RuleParseTreeNode : ParseTreeNode
{
    public readonly CompositeNonterminal Production;
    public readonly Rule Rule;

    public RuleParseTreeNode(Rule rule, CompositeNonterminal production, List<ParseTreeNode> nodes) : base(nodes)
    {
        Rule = rule;
        Production = production;
    }

    public override SourceRange SourceRange
    {
        get
        {
            if (Nodes.Count == 0) return new SourceRange(CharPosition.Undefined, CharPosition.Undefined);
            return new SourceRange(Nodes[0].SourceRange.Start, Nodes[Nodes.Count - 1].SourceRange.End);
        }
    }

    public override void Format(StringBuilder stringBuilder, int indent)
    {
        stringBuilder.AppendLine($"{Rule} -> {Production}: ");
        foreach (var node in Nodes)
        {
            stringBuilder.Append(new string('-', indent * 2));
            node.Format(stringBuilder, indent + 1);
        }
    }
}