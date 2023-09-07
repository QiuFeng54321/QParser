using QParser.Lexer;
using QParser.Parser;

namespace QParser.Generator;

// Statements -> Statement+
// Statement -> Rule = Composites ;
//            | Token = Integer ;
// Composites -> Composite TrailingComposites=( | Composite )+
// Composite -> Symbol+
// Symbol -> Rule
// Symbol -> Token
// Symbol -> ( Composite )
// Symbol -> Symbol +
// Symbol -> Symbol *
// Symbol -> Symbol ?
// Token -> Identifier
// Rule -> < Identifier >
public class ParserGrammarConstructor : GrammarConstructor
{
    public override void Construct()
    {
        var rule = R("Rule")
            .Add(T(DefaultTokenType.LessThan), T(DefaultTokenType.Identifier), T(DefaultTokenType.GreaterThan));
        var token = R("Token").Add(T(DefaultTokenType.Identifier));
        var symbol = R("Symbol");
        symbol
            .Add(rule)
            .Add(token)
            .Add(symbol, T(DefaultTokenType.Plus))
            .Add(symbol, T(DefaultTokenType.Multiply))
            .Add(symbol, T(DefaultTokenType.QuestionMark));
        var composite = ZeroOrMore("Composite", symbol);
        symbol.Add(T(DefaultTokenType.OpenParen), composite, T(DefaultTokenType.CloseParen));
        var composites = R("Composites")
            .Add(composite, ZeroOrMore("TrailingComposites", T(DefaultTokenType.BitOr), composite));
        var stmt = R("Statement")
            .Add(rule, T(DefaultTokenType.Colon), composites)
            .Add(T(DefaultTokenType.Identifier), T(DefaultTokenType.Equal), T(DefaultTokenType.Integer))
            .Add(T(DefaultTokenType.QuestionMark), T(DefaultTokenType.Identifier), T(DefaultTokenType.Equal),
                T(DefaultTokenType.String));
        var stmts = ZeroOrMore("Statements", stmt, T(DefaultTokenType.Semicolon));
        SetEntry(stmts);
    }
}