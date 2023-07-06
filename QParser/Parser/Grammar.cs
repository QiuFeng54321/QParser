using System.Text;
using QParser.Lexer;

namespace QParser.Parser;

public class Grammar
{
    public readonly HashSet<Rule> Rules = new();
    public void AddRule(Rule rule) => Rules.Add(rule);
    public readonly TokenTerminal Epsilon;
    public Rule? EntryRule { set; get; }
    public bool FirstGenerated;
    public bool FollowGenerated;

    public Grammar()
    {
        Epsilon = new(this, TokenType.Epsilon);
    }

    public void GenerateCanBeEmpty()
    {
        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var rule in Rules)
            {
                changed |= rule.GenerateCanBeEmpty();
            }
        }
    }

    public void GenerateFirst()
    {
        if (FirstGenerated) return;
        var changed = true;
        foreach (var rule in Rules)
        {
            rule.GenerateFirstGenerator();
        }

        while (changed)
        {
            changed = false;
            foreach (var rule in Rules)
            {
                foreach (var subRule in rule.FirstGenerator)
                {
                    changed |= subRule.RoundGenerateFirst();
                }

                changed |= rule.RoundGenerateFirst();
            }
        }

        FirstGenerated = true;
    }

    public void GenerateFollowGenerator()
    {
        // Traverse every sub-rule
        foreach (var rule in Rules)
        {
            foreach (var subRule in rule.SubRules)
            {
                switch (subRule)
                {
                    case TokenTerminal: // Skip tokens
                        continue;
                    case CompositeNonterminal compositeNonterminal:
                    {
                        // Iterate backwards so that FIRST set can be generated more efficiently
                        HashSet<TokenType> first = new();
                        for (var i = compositeNonterminal.Components.Length - 1; i >= 1; i--)
                        {
                            // Only keep the FIRST set from the left to the first non-epsilon-deriving component
                            if (!compositeNonterminal.Components[i].CanBeEmpty)
                            {
                                first.Clear();
                            }

                            first.UnionWith(compositeNonterminal.Components[i].First);

                            var previousComponent = compositeNonterminal.Components[i - 1];
                            // Do not add epsilon if FIRST doesn't include one
                            // A -> aBb: FOLLOW(B) += FIRST(b) except epsilon
                            var followShouldHaveEpsilon = previousComponent.Follow
                                .Contains(TokenType.Epsilon);
                            previousComponent.Follow.UnionWith(first);
                            if (!followShouldHaveEpsilon)
                                previousComponent.Follow.Remove(TokenType.Epsilon);
                            // A -> aBb, b =*> epsilon: FOLLOW(B) += FOLLOW(A)
                            if (first.Contains(TokenType.Epsilon))
                            {
                                previousComponent.FollowGenerator.Add(rule);
                            }
                        }

                        // A -> aB: FOLLOW(B) += FOLLOW(A)
                        compositeNonterminal.Components[^1].FollowGenerator.Add(rule);

                        break;
                    }
                    case Rule singleRule:
                        // A -> B: FOLLOW(B) += FOLLOW(A)
                        singleRule.FollowGenerator.Add(rule);
                        break;
                }
            }
        }
    }

    public void GenerateFollow()
    {
        if (FollowGenerated) return;
        EntryRule?.Follow.Add(TokenType.Eof);
        GenerateFollowGenerator();
        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var rule in Rules)
            {
                foreach (var subRule in rule.FollowGenerator)
                {
                    changed |= subRule.RoundGenerateFollow();
                }

                changed |= rule.RoundGenerateFollow();
            }
        }

        FollowGenerated = true;
    }

    public override string ToString()
    {
        return ToString(FirstGenerated, FollowGenerated);
    }

    public string ToString(bool withFirst, bool withFollow)
    {
        var sb = new StringBuilder();
        foreach (var rule in Rules)
        {
            rule.Format(sb, withFirst, withFollow);
        }

        return sb.ToString();
    }
}