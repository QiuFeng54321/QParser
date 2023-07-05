using QParser.Lexer;
using QParser.Parser;

namespace QParser;

public static class Extensions
{
    
    public static NonterminalWithAction A(this Nonterminal nonterminal, Action<Nonterminal> action)
    {
        return new NonterminalWithAction(nonterminal, action);
    }

    public static bool UnionAndCheckChange<T>(this HashSet<T> hashSet, HashSet<T> hashSet2)
    {
        var lengthBefore = hashSet.Count;
        hashSet.UnionWith(hashSet2);
        return lengthBefore != hashSet.Count;
    }
    
}