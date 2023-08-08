using QParser.Lexer;
using QParser.Parser;

namespace QParser.Test.TestParsers;

public class DragonBookEg0455GrammarConstructor : GrammarConstructor
{
    public override void Construct()
    {
        var s = R("S");
        var c = R("C");
        s.Add(C(c, c));
        c.Add(C(T(DefaultTokenType.OpenParen), c));
        c.Add(C(T(DefaultTokenType.CloseParen)));
        SetEntry(s);
    }
}