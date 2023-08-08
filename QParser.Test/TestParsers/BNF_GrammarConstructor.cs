using QParser.Lexer;
using QParser.Parser;

namespace QParser.Test.TestParsers;

public class BNF_GrammarConstructor : GrammarConstructor
{
    public override void Construct()
    {
        var stmts = R("Statements");
        var stmt = R("Statement");
        var composite = R("Composite");
        var composites = R("Composites");
        var symbol = R("Symbol");
        var rule = R("Rule");
        var token = R("Token");
        rule.Add(T(DefaultTokenType.LessThan), T(DefaultTokenType.Identifier),
            T(DefaultTokenType.GreaterThan));
        token.Add(T(DefaultTokenType.Identifier));
        symbol.Add(rule).Add(token);
        composite.Add(composite, symbol).Add(symbol);
        composites.Add(composites, T(DefaultTokenType.BitOr), composite).Add(composite);
        stmt.Add(rule, T(DefaultTokenType.Equal), composites, T(DefaultTokenType.Semicolon));
        stmt.Add(token, T(DefaultTokenType.Equal), T(DefaultTokenType.Integer), T(DefaultTokenType.Semicolon));
        stmts.Add(stmts, stmt).Add(stmt);
        SetEntry(stmts);
    }
}