using QParser.Lexer;

namespace QParser.Parser;

public abstract class GrammarConstructor
{
    public readonly Grammar Grammar = new();
    protected CompositeNonterminal Epsilon => Grammar.Epsilon;
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

    protected Rule R(string name, params CompositeNonterminal[] components)
    {
        var rule = new Rule(Grammar, name, components.ToHashSet());
        Grammar.AddRule(rule);
        return rule;
    }

    protected void SetEntry(Rule rule)
    {
        Grammar.EntryRule = R("$ENTRY", C(rule, T(TokenConstants.Eof)));
        Grammar.AddRule(Grammar.EntryRule);
    }
}