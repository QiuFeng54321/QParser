using System.Collections.Generic;
using System.Text;

namespace QParser.Parser;

public abstract class ParseTreeNode
{
    public readonly List<ParseTreeNode> Nodes;
    public object? Data;

    protected ParseTreeNode(List<ParseTreeNode> nodes)
    {
        Nodes = nodes;
    }

    public abstract SourceRange SourceRange { get; }
    public abstract void Format(StringBuilder stringBuilder, int indent);

    public override string ToString()
    {
        var sb = new StringBuilder();
        Format(sb, 1);
        return sb.ToString();
    }
}