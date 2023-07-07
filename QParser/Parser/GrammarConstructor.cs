using QParser.Lexer;

namespace QParser.Parser;

public abstract class GrammarConstructor
{
    public readonly Grammar Grammar = new();
    public CompositeNonterminal Epsilon => Grammar.Epsilon;
    public abstract void Construct();

    public TokenTerminal T(TokenType tokenType)
    {
        return new TokenTerminal(Grammar, tokenType);
    }

    public CompositeNonterminal C(params Nonterminal[] components)
    {
        return new CompositeNonterminal(Grammar, components);
    }

    public Rule R(string name, params CompositeNonterminal[] components)
    {
        var rule = new Rule(Grammar, name, components.ToHashSet());
        Grammar.AddRule(rule);
        return rule;
    }

    public Rule Entry(Rule rule)
    {
        return Grammar.EntryRule = rule;
    }
}