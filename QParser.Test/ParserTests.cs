using System.Collections;
using QParser.Lexer;
using QParser.Lexer.Tokens;
using QParser.Parser;
using QParser.Test.TestParsers;

namespace QParser.Test;

public class ParserTestsFixtureData
{
    public static IEnumerable FixtureParams
    {
        get
        {
            yield return new TestFixtureData(new TestParser2GrammarConstructor(), new ParserTestCase(new[]
            {
                new IntegerToken("3"), new Token(TokenType.Plus, "+"), new RealToken("4.5")
            }));
            yield return new TestFixtureData(new TestParser2GrammarConstructor(), new ParserTestCase(new Token[]
            {
                new IdentifierToken("??")
            }, ParseResult.Failure));
        }
    }
}

public enum ParseResult
{
    Success,
    Failure
}

public record ParserTestCase(Token[] Tokens, ParseResult Result = ParseResult.Success);

[TestFixtureSource(typeof(ParserTestsFixtureData), nameof(ParserTestsFixtureData.FixtureParams))]
public class ParserTests
{
    [SetUp]
    public void Setup()
    {
        _grammarConstructor.Construct();
        _grammarConstructor.Grammar.GenerateCanBeEmpty();
    }

    private readonly GrammarConstructor _grammarConstructor;
    private readonly ParserTestCase _parserTestCase;

    public ParserTests(GrammarConstructor grammarConstructor, ParserTestCase parserTestCase)
    {
        _grammarConstructor = grammarConstructor;
        _parserTestCase = parserTestCase;
    }

    [Test]
    public void OutputGrammar()
    {
        Console.WriteLine("Original ------------------------------");
        Console.WriteLine(_grammarConstructor.Grammar);
    }

    [Test]
    public void ParseLL1()
    {
        _grammarConstructor.Grammar.GenerateFirst();
        _grammarConstructor.Grammar.GenerateFollow();
        var canBeLL1 = _grammarConstructor.Grammar.CanBeLL1();
        if (canBeLL1.IsFailed) Assert.Pass();
        var parser = new LL1Parser(_grammarConstructor.Grammar);
        parser.DumpTable();
        if (_parserTestCase.Result is ParseResult.Success)
        {
            foreach (var token in _parserTestCase.Tokens) parser.Feed(token);
            parser.Feed(new Token(TokenType.Eof, "$"));

            Assert.That(parser.Accepted, Is.True);
        }
        else
        {
            Assert.Catch(() =>
            {
                foreach (var token in _parserTestCase.Tokens) parser.Feed(token);
            });
        }
    }
}