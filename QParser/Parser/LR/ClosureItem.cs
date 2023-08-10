using System.Collections.Generic;
using System.Text;
using QParser.Lexer;

namespace QParser.Parser.LR;

public class ClosureItem
{
    public ClosureItem(Rule rule, CompositeNonterminal production, int index,
        int lookahead = TokenConstants.Unknown)
    {
        Rule = rule;
        Production = production;
        Index = index;
        Lookahead = lookahead;
    }

    public Nonterminal? AfterDot => Index >= Production.Components.Length ? null : Production.Components[Index];
    public Nonterminal? BeforeDot => Index == 0 ? null : Production.Components[Index - 1];

    public bool IsLR1 => Lookahead is not TokenConstants.Unknown;

    public static IEqualityComparer<ClosureItem> LookaheadIrrelevantComparer { get; } =
        new LookaheadIrrelevantEqualityComparer();

    public static IEqualityComparer<ClosureItem> ClosureItemComparer { get; } = new ClosureItemEqualityComparer();

    public Rule Rule { get; }
    public CompositeNonterminal Production { get; }
    public int Index { get; }
    public int Lookahead { get; }

    public HashSet<int> FirstAfterAfterDot()
    {
        if (Index + 1 >= Production.Components.Length) return new HashSet<int> { Lookahead };
        var first = new HashSet<int>();
        for (var i = Index + 1; i < Production.Components.Length; i++)
        {
            first.UnionWith(Production.Components[i].First);
            if (!Production.Components[i].CanBeEmpty) break;
            if (i + 1 == Production.Components.Length && IsLR1) first.Add(Lookahead);
        }

        return first;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(Rule.Name).Append(" -> ");
        for (var i = 0; i < Production.Components.Length; i++)
        {
            if (i == Index) sb.Append(". ");
            sb.Append(Production.Components[i].Name);
            if (i != Production.Components.Length - 1) sb.Append(' ');
        }

        if (Index == Production.Components.Length) sb.Append(" .");
        if (IsLR1)
        {
            sb.Append(" {");
            sb.Append(string.Join(", ", Lookahead));
            sb.Append('}');
        }

        return sb.ToString();
    }

    public void Deconstruct(out Rule rule, out CompositeNonterminal production, out int index, out int lookahead)
    {
        rule = Rule;
        production = Production;
        index = Index;
        lookahead = Lookahead;
    }

    public override int GetHashCode()
    {
        return ClosureItemComparer.GetHashCode(this);
    }

    private sealed class LookaheadIrrelevantEqualityComparer : IEqualityComparer<ClosureItem>
    {
        public bool Equals(ClosureItem x, ClosureItem y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Rule.Equals(y.Rule) && x.Production.Equals(y.Production) && x.Index == y.Index;
        }

        public int GetHashCode(ClosureItem obj)
        {
            unchecked
            {
                var hashCode = obj.Rule.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Production.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Index;
                return hashCode;
            }
        }
    }

    private sealed class ClosureItemEqualityComparer : IEqualityComparer<ClosureItem>
    {
        public bool Equals(ClosureItem x, ClosureItem y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Rule.Equals(y.Rule) && x.Production.Equals(y.Production) && x.Index == y.Index &&
                   x.Lookahead == y.Lookahead;
        }

        public int GetHashCode(ClosureItem obj)
        {
            unchecked
            {
                var hashCode = obj.Rule.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Production.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Index;
                hashCode = (hashCode * 397) ^ obj.Lookahead;
                return hashCode;
            }
        }
    }
}