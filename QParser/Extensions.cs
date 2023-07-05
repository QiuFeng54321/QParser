using QParser.Lexer;
using QParser.Parser;

namespace QParser;

public static class Extensions
{
    
    public static NonterminalWithAction A(this Nonterminal nonterminal, Action<Nonterminal> action)
    {
        return new NonterminalWithAction(nonterminal, action);
    }
    
}