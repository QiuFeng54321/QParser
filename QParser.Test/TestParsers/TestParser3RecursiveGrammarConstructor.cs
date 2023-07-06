using QParser.Lexer;
using QParser.Parser;

namespace QParser.Test.TestParsers;

public class TestParser3RecursiveGrammarConstructor : GrammarConstructor
{
    /// <summary>
    /// S -> A
    /// A -> A B
    /// A -> B A
    /// A -> id
    /// B -> C A
    /// B -> C
    /// B -> int
    /// C -> B C
    /// C -> eps
    /// </summary>
    public override void Construct()
    {
        var program = Entry(R("Program"));
        var a = R("A");
        var b = R("B");
        var c = R("C");
        program.Add(a);
        a.Add(a, b)
            .Add(b, a)
            .Add(T(TokenType.Identifier));
        b.Add(c, a)
            .Add(c)
            .Add(T(TokenType.Integer));
        c.Add(C(b, c))
            .Add(Epsilon);
    }
}