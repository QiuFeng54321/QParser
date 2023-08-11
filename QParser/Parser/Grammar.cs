using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentResults;
using QParser.Parser.LL;

namespace QParser.Parser;

public class Grammar
{
    public readonly CompositeNonterminal Epsilon, Eof;
    public readonly LL1ParseTable LL1ParseTable = new();
    public readonly Dictionary<string, Rule> Rules = new();
    public bool CanBeEmptyGenerated;
    public bool FirstGenerated;
    public bool FollowGenerated;
    public bool NonKernelItemsGenerated;

    public Grammar()
    {
        Epsilon = new CompositeNonterminal(this, new TokenTerminal(this, TokenConstants.Epsilon));
        Eof = new CompositeNonterminal(this, new TokenTerminal(this, TokenConstants.Eof));
    }

    public Rule? EntryRule { set; get; }

    public Rule AddOrMergeRule(Rule rule)
    {
        if (!Rules.TryAdd(rule.Name, rule))
        {
            Rules[rule.Name].SubRules.UnionWith(rule.SubRules);
            return Rules[rule.Name];
        }

        return rule;
    }

    public void GenerateCanBeEmpty()
    {
        if (CanBeEmptyGenerated) return;
        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var (name, rule) in Rules) changed |= rule.GenerateCanBeEmpty();
        }

        CanBeEmptyGenerated = true;
    }

    public void GenerateFirst()
    {
        if (FirstGenerated) return;
        GenerateCanBeEmpty();
        var changed = true;
        foreach (var (name, rule) in Rules) rule.GenerateFirstGenerator();

        while (changed)
        {
            changed = false;
            foreach (var (name, rule) in Rules)
            {
                foreach (var subRule in rule.FirstGenerator) changed |= subRule.RoundGenerateFirst();

                changed |= rule.RoundGenerateFirst();
            }
        }

        FirstGenerated = true;
    }

    private void GenerateFollowGenerator()
    {
        // Traverse every sub-rule
        foreach (var (name, rule) in Rules)
        foreach (var subRule in rule.SubRules)
        {
            // Iterate backwards so that FIRST set can be generated more efficiently
            HashSet<int> first = new();
            for (var i = subRule.Components.Length - 1; i >= 1; i--)
            {
                // Only keep the FIRST set from the left to the first non-epsilon-deriving component
                if (!subRule.Components[i].CanBeEmpty) first.Clear();

                first.UnionWith(subRule.Components[i].First);

                var previousComponent = subRule.Components[i - 1];
                // Do not add epsilon if FIRST doesn't include one
                // A -> aBb: FOLLOW(B) += FIRST(b) except epsilon
                var followShouldHaveEpsilon = previousComponent.Follow
                    .Contains(TokenConstants.Epsilon);
                previousComponent.Follow.UnionWith(first);
                if (!followShouldHaveEpsilon)
                    previousComponent.Follow.Remove(TokenConstants.Epsilon);
                // A -> aBb, b =*> epsilon: FOLLOW(B) += FOLLOW(A)
                if (first.Contains(TokenConstants.Epsilon)) previousComponent.FollowGenerator.Add(rule);
            }

            // A -> aB: FOLLOW(B) += FOLLOW(A)
            subRule.Components[subRule.Components.Length - 1].FollowGenerator.Add(rule);
        }
    }

    public void GenerateFollow()
    {
        if (FollowGenerated) return;
        GenerateFirst();
        // EntryRule?.Follow.Add(TokenConstants.Eof);
        GenerateFollowGenerator();
        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var (name, rule) in Rules)
            {
                foreach (var subRule in rule.FollowGenerator) changed |= subRule.RoundGenerateFollow();

                changed |= rule.RoundGenerateFollow();
            }
        }

        FollowGenerated = true;
    }

    public void GenerateNonKernelItems()
    {
        if (NonKernelItemsGenerated) return;
        GenerateFollow();
        bool changed;
        do
        {
            changed = false;
            foreach (var (name, rule) in Rules)
            {
                changed |= rule.GenerateNonKernelItems();
                foreach (var subRule in rule.SubRules)
                    changed |= subRule.GenerateNonKernelItems();
            }
        } while (changed);

        NonKernelItemsGenerated = true;
    }

    public Result CanBeLL1()
    {
        foreach (var (name, rule) in Rules)
        {
            if (rule.SubRules.Count == 0) return Result.Fail($"Empty rule: {rule}"); // ???
            HashSet<int> overlappingFirst = new();
            var firstIteration = true;
            var hasEmpty = false;
            CompositeNonterminal? overlappingSubRule = null;
            foreach (var subRule in rule.SubRules)
            {
                if (firstIteration)
                {
                    overlappingFirst.UnionWith(subRule.First);
                    firstIteration = false;
                }
                else
                {
                    overlappingFirst.IntersectWith(subRule.First);
                }

                if (subRule.CanBeEmpty)
                {
                    if (hasEmpty) return Result.Fail($"More than one production can produce epsilon in {rule}");
                    hasEmpty = true;
                }
                else
                {
                    if (subRule.First.Intersect(rule.Follow).Any()) overlappingSubRule = subRule;
                }
            }

            if (hasEmpty && overlappingSubRule != null)
                return Result.Fail(
                    $"FIRST({overlappingSubRule}) = {{{string.Join(", ", overlappingSubRule.First)}}} overlaps FOLLOW({rule}) = {{{string.Join(", ", rule.Follow)}}}");
            if (overlappingFirst.Count != 0 && rule.SubRules.Count != 1)
                return Result.Fail($"Overlapping FIRST set in {rule}");
        }

        return Result.Ok();
    }

    public override string ToString()
    {
        return ToString(FirstGenerated, FollowGenerated);
    }

    public string ToString(bool withFirst, bool withFollow)
    {
        var sb = new StringBuilder();
        foreach (var (name, rule) in Rules) rule.Format(sb, withFirst, withFollow);

        return sb.ToString();
    }

    public void GenerateAll()
    {
        GenerateCanBeEmpty();
        GenerateFirst();
        GenerateFollow();
        GenerateNonKernelItems();
    }
}