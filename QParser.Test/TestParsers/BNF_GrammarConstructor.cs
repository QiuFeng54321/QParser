using QParser.Lexer;
using QParser.Parser;

namespace QParser.Test.TestParsers;

public class BNF_GrammarConstructor : GrammarConstructor
{
    public override void Construct()
    {
        var rule = R("Rule")
            .Add(T(DefaultTokenType.LessThan), T(DefaultTokenType.Identifier), T(DefaultTokenType.GreaterThan));
        var token = R("Token").Add(T(DefaultTokenType.Identifier));
        var symbol = R("Symbol")
            .Add(rule)
            .Add(token);
        var composite = ZeroOrMore("Composite", symbol);
        var composites = R("Composites")
            .Add(composite, ZeroOrMore("TrailingComposites", T(DefaultTokenType.BitOr), composite));
        var stmt = R("Statement")
            .Add(rule, T(DefaultTokenType.Equal), composites, T(DefaultTokenType.Semicolon))
            .Add(token, T(DefaultTokenType.Equal), T(DefaultTokenType.Integer), T(DefaultTokenType.Semicolon));
        var stmts = ZeroOrMore("Statements", stmt);
        SetEntry(stmts);
    }
}