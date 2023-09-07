using System;
using System.Collections.Generic;

namespace QParser.Parser;

public abstract class GrammarConstructor
{
    public readonly Grammar Grammar = new();

    private int _tempRuleId;
    protected CompositeNonterminal Epsilon => Grammar.Epsilon;
    private int TempRuleId => _tempRuleId++;
    public string TempRuleName => $"$TMP{TempRuleId}";
    public abstract void Construct();

    public TokenTerminal T(int tokenType)
    {
        return new TokenTerminal(Grammar, tokenType);
    }

    public TokenTerminal T<TToken>(TToken tokenType) where TToken : Enum
    {
        return T(tokenType.ToInt());
    }

    public CompositeNonterminal C(params Nonterminal[] components)
    {
        return new CompositeNonterminal(Grammar, components);
    }

    public Rule R(string name, bool generated, HashSet<CompositeNonterminal> components)
    {
        var rule = new Rule(Grammar, name, components, generated);
        return Grammar.AddOrMergeRule(rule);
    }

    public Rule R(string name, bool generated, params CompositeNonterminal[] components)
    {
        return R(name, generated, new HashSet<CompositeNonterminal>(components));
    }

    public Rule R(string name, params CompositeNonterminal[] components)
    {
        return R(name, false, components);
    }

    public void SetEntry(Rule rule)
    {
        Grammar.EntryRule = R("$ENTRY", C(rule, T(TokenConstants.Eof)));
    }

    private Rule MakeTempRule()
    {
        return R(TempRuleName, true);
    }

    public Rule OneOrMany(string name, params Nonterminal[] components)
    {
        if (components.Length == 0) throw new FormatException("Length of components must be at least one!");
        var ruleGroup = components.Length > 1 ? MakeTempRule().Add(components) : components[0];
        if (ruleGroup is OneOrManyRule alreadyOneOrManyRule) return alreadyOneOrManyRule;
        if (ruleGroup is Rule someRule && someRule.SubRules.Contains(Epsilon)) return ZeroOrMore(name, components);
        var macroRule = new OneOrManyRule(Grammar, name, new HashSet<CompositeNonterminal>(), true);
        macroRule.Add(ruleGroup).Add(macroRule, ruleGroup);
        return Grammar.AddOrMergeRule(macroRule);
    }

    public Rule ZeroOrMore(string name, params Nonterminal[] components)
    {
        if (components.Length == 0) throw new FormatException("Length of components must be at least one!");
        var ruleGroup = components.Length > 1 ? MakeTempRule().Add(components) : components[0];
        if (ruleGroup is ZeroOrMoreRule alreadyZeroOrMoreRule) return alreadyZeroOrMoreRule;
        var macroRule = new ZeroOrMoreRule(Grammar, name, new HashSet<CompositeNonterminal>(), true);
        macroRule.Add(macroRule, ruleGroup);
        macroRule.Add(Epsilon);
        return Grammar.AddOrMergeRule(macroRule);
    }

    public Rule Optional(string name, params Nonterminal[] components)
    {
        var ruleGroup = new Rule(Grammar, name, new HashSet<CompositeNonterminal>(), true);
        ruleGroup.Add(components).Add(Epsilon);
        return ruleGroup;
    }
}