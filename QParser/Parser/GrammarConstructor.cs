using QParser.Lexer;

namespace QParser.Parser;

public abstract class GrammarConstructor
{
    public readonly Grammar Grammar = new();

    private int _tempRuleId;
    protected CompositeNonterminal Epsilon => Grammar.Epsilon;
    private int TempRuleId => _tempRuleId++;
    private string TempRuleName => $"$TMP{TempRuleId}";
    public abstract void Construct();

    protected TokenTerminal T(int tokenType)
    {
        return new TokenTerminal(Grammar, tokenType);
    }

    protected TokenTerminal T<TToken>(TToken tokenType) where TToken : Enum
    {
        return T(tokenType.ToInt());
    }

    protected CompositeNonterminal C(params Nonterminal[] components)
    {
        return new CompositeNonterminal(Grammar, components);
    }

    protected Rule R(string name, bool generated, params CompositeNonterminal[] components)
    {
        var rule = new Rule(Grammar, name, components.ToHashSet(), generated);
        Grammar.AddRule(rule);
        return rule;
    }

    protected Rule R(string name, params CompositeNonterminal[] components)
    {
        return R(name, false, components);
    }

    protected void SetEntry(Rule rule)
    {
        Grammar.EntryRule = R("$ENTRY", C(rule, T(TokenConstants.Eof)));
        Grammar.AddRule(Grammar.EntryRule);
    }

    private Rule MakeTempRule()
    {
        return R(TempRuleName, true);
    }

    protected Rule OneOrMany(string name, params Nonterminal[] components)
    {
        if (components.Length == 0) throw new FormatException("Length of components must be at least one!");
        var ruleGroup = components.Length > 1 ? MakeTempRule().Add(components) : components[0];
        var macroRule = new OneOrManyRule(Grammar, name, new HashSet<CompositeNonterminal>(), true);
        Grammar.AddRule(macroRule);
        macroRule.Add(ruleGroup).Add(macroRule, ruleGroup);
        return macroRule;
    }

    protected Rule ZeroOrMore(string name, params Nonterminal[] components)
    {
        if (components.Length == 0) throw new FormatException("Length of components must be at least one!");
        var ruleGroup = components.Length > 1 ? MakeTempRule().Add(components) : components[0];
        var macroRule = new ZeroOrMoreRule(Grammar, name, new HashSet<CompositeNonterminal>(), true);
        Grammar.AddRule(macroRule);
        macroRule.Add(macroRule, ruleGroup);
        macroRule.Add(Epsilon);
        return macroRule;
    }

    protected Rule Optional(string name, params Nonterminal[] components)
    {
        var ruleGroup = new Rule(Grammar, name, new HashSet<CompositeNonterminal>(), true);
        ruleGroup.Add(components).Add(Epsilon);
        return ruleGroup;
    }
}