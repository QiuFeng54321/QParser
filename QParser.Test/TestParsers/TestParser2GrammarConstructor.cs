using QParser.Lexer;
using QParser.Parser;

namespace QParser.Test.TestParsers;

public class TestParser2GrammarConstructor : GrammarConstructor
{
    /// <summary>
    /// S -> A
    /// A -> M A'
    /// A' -> + M A'
    /// A' -> eps
    /// M -> E M'
    /// M' -> * E M'
    /// M' -> eps
    /// E -> ( A )
    /// E -> num
    /// </summary>
    public override void Construct()
    {
        var mult = R("Mult");
        var mult2 = R("Mult2");
        var add = R("Addition");
        var add2 = R("Addition2");
        var e = R("E");
        SetEntry(add);
        add.Add(mult, add2);
        add2.Add(T(DefaultTokenType.Plus), mult, add2)
            .Add(Epsilon);
        mult.Add(e, mult2);
        mult2.Add(T(DefaultTokenType.Multiply), e, mult2)
            .Add(Epsilon);
        e.Add(T(DefaultTokenType.OpenParen), add, T(DefaultTokenType.CloseParen))
            .Add(T(DefaultTokenType.Integer))
            .Add(T(DefaultTokenType.Real));
    }
}