using QParser.Lexer;

namespace QParser.Parser;

public abstract class GrammarConstructor
{
    public readonly Grammar Grammar = new();
    public abstract void Construct();
    public TokenTerminal T(TokenType tokenType) => new(Grammar, tokenType);
    public CompositeNonterminal C(params Nonterminal[] components) => new(Grammar, components);
    public Rule R(string name, params Nonterminal[] components)
    {
        var rule = new Rule(Grammar, name, components.ToHashSet());
        Grammar.AddRule(rule);
        return rule;
    }

    public TokenTerminal Epsilon => Grammar.Epsilon;
}