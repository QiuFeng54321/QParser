using QParser.Lexer;
using QParser.Parser;

namespace QParser.Test.TestParsers;

public class DragonBook4_2_1GrammarConstructor : GrammarConstructor
{
    /// <summary>
    /// P -> S
    /// S -> S S +
    /// S -> S S *
    /// S -> a
    /// </summary>
    public override void Construct()
    {
        var s = R("S");
        SetEntry(s);
        s.Add(s, s, T(DefaultTokenType.Plus))
            .Add(s, s, T(DefaultTokenType.Multiply))
            .Add(T(DefaultTokenType.Integer));
    }
}