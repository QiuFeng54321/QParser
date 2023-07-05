using QParser.Lexer;
using QParser.Parser;

namespace QParser.Test.TestParsers;

public class TestParser3RecursiveGrammarConstructor : GrammarConstructor
{
    /// <summary>
    /// S -> A
    /// A -> A B
    /// A -> id
    /// B -> C A
    /// C -> B C
    /// C -> eps
    /// </summary>
    public override void Construct()
    {
        var program = R("Program");
        var a = R("A");
        var b = R("B");
        var c = R("C");
        program.Add(a);
        a.Add(C(a, b))
            .Add(T(TokenType.Identifier));
        b.Add(C(c, a));
        c.Add(C(b, c))
            .Add(Epsilon);
    }
}