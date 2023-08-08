using QParser.Lexer;
using QParser.Parser;

namespace QParser.Test.TestParsers;

public class TestParser1GrammarConstructor : GrammarConstructor
{
    public override void Construct()
    {
        var decl = R("Declare Statement",
            C(T(DefaultTokenType.Declare), T(DefaultTokenType.Identifier), T(DefaultTokenType.Colon),
                T(DefaultTokenType.Identifier))
        );
        SetEntry(decl);
    }
}