using QParser.Lexer;
using QParser.Parser;

namespace QParser.Test.TestParsers;

public class DragonBook4_2_2eGrammarConstructor : GrammarConstructor
{
    /// <summary>
    /// P -> S
    /// S -> ( L ) | a
    /// L -> L , S | S
    /// </summary>
    public override void Construct()
    {
        var s = R("S");
        var l = R("L");
        SetEntry(s);
        s.Add(T(DefaultTokenType.OpenParen), l, T(DefaultTokenType.CloseParen))
            .Add(T(DefaultTokenType.Integer));
        l.Add(l, T(DefaultTokenType.Comma), s)
            .Add(s);
    }
}