using QParser.Lexer;
using QParser.Parser;

namespace QParser.Test.TestParsers;

public class TestParser1GrammarConstructor : GrammarConstructor
{
    public override void Construct()
    {
        var decl = R( "Declare Statement",
            C(T(TokenType.Declare), T(TokenType.Identifier), T(TokenType.Colon), T(TokenType.Identifier))
            );
       
    }
}