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
    // public override void Construct()
    // {
    //     var rule = R("Rule")
    //         .Add(T(DefaultTokenType.LessThan), T(DefaultTokenType.Identifier), T(DefaultTokenType.GreaterThan));
    //     var token = R("Token").Add(T(DefaultTokenType.Identifier));
    //     var symbol = R("Symbol");
    //     symbol
    //         .Add(rule)
    //         .Add(token);
    //         // .Add(symbol, T(DefaultTokenType.Plus))
    //         // .Add(symbol, T(DefaultTokenType.Multiply))
    //         // .Add(symbol, T(DefaultTokenType.QuestionMark));
    //     var composite = ZeroOrMore("Composite", symbol);
    //     symbol.Add(T(DefaultTokenType.OpenParen), composite, T(DefaultTokenType.CloseParen));
    //     var composites = R("Composites")
    //         .Add(composite, ZeroOrMore("TrailingComposites", T(DefaultTokenType.BitOr), composite));
    //     var stmt = R("Statement")
    //         .Add(rule, T(DefaultTokenType.Equal), composites, T(DefaultTokenType.Semicolon))
    //         .Add(token, T(DefaultTokenType.Equal), T(DefaultTokenType.Integer), T(DefaultTokenType.Semicolon));
    //     var stmts = ZeroOrMore("Statements", stmt);
    //     SetEntry(stmts);
    // }
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