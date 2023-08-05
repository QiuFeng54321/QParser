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
        s.Add(T(TokenType.OpenParen), l, T(TokenType.CloseParen))
            .Add(T(TokenType.Integer));
        l.Add(l, T(TokenType.Comma), s)
            .Add(s);
    }
}