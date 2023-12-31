using System.Collections.Generic;
using System.Linq;
using System.Text;
using QParser.Parser.LR;

namespace QParser.Parser;

public class Rule : Nonterminal
{
    public readonly bool IsGenerated;
    public readonly HashSet<CompositeNonterminal> SubRules;

    public Rule(Grammar grammar, string name, HashSet<CompositeNonterminal> subRules, bool isGenerated = false) :
        base(grammar)
    {
        SubRules = subRules;
        IsGenerated = isGenerated;
        Name = name;
    }

    /// <summary>
    ///     A rule is redundant <em>iff</em> it contains only a single sub-rule, and the sub-rule contains a single rule
    ///     In this case, the rule is equivalent to the rule contained in the only sub-rule
    /// </summary>
    public bool IsRedundant => SubRules.Count == 1 && SubRules.First().IsSingleRule;

    protected override void InternalGenerateFirstGenerator()
    {
        foreach (var subRule in SubRules)
        {
            subRule.GenerateFirstGenerator();
            if (subRule.IsSingleToken)
                First.UnionWith(subRule.First);
            else
                FirstGenerator.Add(subRule);
        }
    }

    public override bool GenerateNonKernelItems()
    {
        var prevLen = NonKernelItems.Count;
        foreach (var subRule in SubRules)
        {
            NonKernelItems.Add(new ClosureItem(this, subRule, 0));
            NonKernelItems.UnionWith(subRule.NonKernelItems);
        }

        return prevLen != NonKernelItems.Count;
    }

    protected override void InternalGenerateCanBeEmpty()
    {
        foreach (var subRule in SubRules) subRule.GenerateCanBeEmpty();
        foreach (var r in SubRules)
        {
            if (!r.CanBeEmpty) continue;
            // A sub-rule is certain to allow empty 
            CanBeEmpty = CanBeEmptyGenerated = true;
            break;
        }
    }

    public Rule Add(CompositeNonterminal nonterminal)
    {
        SubRules.Add(nonterminal);
        return this;
    }

    public Rule Add(params Nonterminal[] nonterminal)
    {
        SubRules.Add(new CompositeNonterminal(Grammar, nonterminal));
        return this;
    }

    public virtual ParseTreeNode MakeNode(CompositeNonterminal production, List<ParseTreeNode> children)
    {
        return new RuleParseTreeNode(this, production, children);
    }


    public void Format(StringBuilder sb, bool withFirst, bool withFollow)
    {
        sb.AppendLine();
        sb.Append($"Rule {Name}");
        sb.Append(':');
        AppendFirstFollow(sb, this, withFirst, withFollow);

        sb.AppendLine();
        foreach (var subRule in SubRules)
        {
            sb.Append($"{Name} -> {subRule};");
            // FOLLOW is not calculated for sub-rules
            AppendFirstFollow(sb, subRule, withFirst, false);
            sb.AppendLine();
        }
    }

    private static void AppendFirstFollow(StringBuilder sb, Nonterminal rule, bool withFirst, bool withFollow)
    {
        if (rule.CanBeEmpty) sb.Append(" (Can be empty)");
        if (withFirst) sb.Append($" FIRST = {{{string.Join(", ", rule.First)}}}");
        if (withFollow) sb.Append($" FOLLOW = {{{string.Join(", ", rule.Follow)}}}");
    }
}