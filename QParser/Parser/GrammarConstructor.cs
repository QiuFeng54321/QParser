using QParser.Lexer;

namespace QParser.Parser;

public abstract class GrammarConstructor
{
    public readonly Grammar Grammar = new();
    public abstract void Construct();
    public TokenSubRule T(TokenType tokenType) => new(Grammar, tokenType);
    public CompositeSubRule C(params SubRule[] components) => new(Grammar, components);
    public Rule R(string name, params SubRule[] components)
    {
        var rule = new Rule(Grammar, name, components.ToList());
        Grammar.AddRule(rule);
        return rule;
    }
}