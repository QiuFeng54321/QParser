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
        var program = Entry(R("Program"));
        var s = R("S");
        program.Add(s);
        s.Add(T(TokenType.Plus), s, T(TokenType.Minus), s)
            .Add(T(TokenType.Minus), s, T(TokenType.Plus), s)
            .Add(Epsilon);
    }
}