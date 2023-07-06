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
        var program = Entry(R("Program"));
        var s = R("S");
        program.Add(s);
        s.Add(C(s, s, T(TokenType.Plus)))
            .Add(C(s, s, T(TokenType.Multiply)))
            .Add(T(TokenType.Integer));
    }
}