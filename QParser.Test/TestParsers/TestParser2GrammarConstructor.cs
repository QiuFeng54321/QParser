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
        var program = R("Program");
        var mult = R("Mult");
        var mult2 = R("Mult2");
        var add = R("Addition");
        var add2 = R("Addition2");
        var e = R("E");
        program.Add(add);
        add.Add(C(mult, add2));
        add2.Add(C(T(TokenType.Plus), mult, add2))
            .Add(Epsilon);
        mult.Add(C(e, mult2));
        mult2.Add(C(T(TokenType.Multiply), e, mult2))
            .Add(Epsilon);
        e.Add(C(T(TokenType.OpenParen), add, T(TokenType.CloseParen)))
            .Add(T(TokenType.Integer))
            .Add(T(TokenType.Real));
    }
}