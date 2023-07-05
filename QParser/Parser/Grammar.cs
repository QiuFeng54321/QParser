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
        foreach (var rule in Rules)
        {
            foreach (var subRule in rule.SubRules)
            {
                switch (subRule)
                {
                    case TokenTerminal:
                        continue;
                    case CompositeNonterminal { Components.Length: 1 } compositeNonterminal:
                        compositeNonterminal.FollowGenerator.Add(rule);
                        continue;
                    case CompositeNonterminal compositeNonterminal:
                    {
                        for (var i = 1; i < compositeNonterminal.Components.Length; i++)
                        {
                            HashSet<TokenType> first = new();
                            for (var j = i; j < compositeNonterminal.Components.Length; j++)
                            {
                                first.UnionWith(compositeNonterminal.Components[j].First);
                                if (!compositeNonterminal.Components[j].CanBeEmpty) break;
                            }

                            if (first.Contains(TokenType.Epsilon))
                            {
                                compositeNonterminal.Components[i - 1].FollowGenerator.Add(rule);
                            }
                            else
                            {
                                compositeNonterminal.Components[i - 1].Follow.UnionWith(first);
                            }

                            if (i == compositeNonterminal.Components.Length - 1)
                            {
                                compositeNonterminal.Components[i].FollowGenerator.Add(rule);
                            }
                        }

                        break;
                    }
                    case Rule singleRule:
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