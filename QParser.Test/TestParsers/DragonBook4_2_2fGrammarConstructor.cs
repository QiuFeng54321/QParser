using QParser.Lexer;
using QParser.Parser;

namespace QParser.Test.TestParsers;

public class DragonBook4_2_2fGrammarConstructor : GrammarConstructor
{
    /// <summary>
    /// P -> S
    /// S -> a S b S
    /// S -> b S a S
    /// S -> eps
    /// </summary>
    public override void Construct()
    {
        var s = R("S");
        SetEntry(s);
        s.Add(T(DefaultTokenType.Plus), s, T(DefaultTokenType.Minus), s)
            .Add(T(DefaultTokenType.Minus), s, T(DefaultTokenType.Plus), s)
            .Add(Epsilon);
    }
}