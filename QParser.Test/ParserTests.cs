using System.Collections;
using System.Text;
using QParser.Lexer;
using QParser.Parser;
using QParser.Test.TestParsers;

namespace QParser.Test;

public class ParserTestsFixtureData
{
    public static IEnumerable FixtureParams
    {
        get
        {
            yield return new TestFixtureData(new TestParser2GrammarConstructor(), new ParserTestCase("3+4.5"));
            yield return new TestFixtureData(new TestParser2GrammarConstructor(),
                new ParserTestCase("(3 + 5) * 2 + 3 * 9"));
            yield return new TestFixtureData(new TestParser2GrammarConstructor(),
                new ParserTestCase("??", ParseResult.Failure));
        }
    }
}

public enum ParseResult
{
    Success,
    Failure
}

public record ParserTestCase(string Input, ParseResult Result = ParseResult.Success);

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
    private readonly QLexer _qLexer;

    public ParserTests(GrammarConstructor grammarConstructor, ParserTestCase parserTestCase)
    {
        _grammarConstructor = grammarConstructor;
        _parserTestCase = parserTestCase;
        _qLexer = new QLexer(new MemoryStream(Encoding.UTF8.GetBytes(parserTestCase.Input)), "TestFile");
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
            foreach (var token in _qLexer)
            {
                if (token.Ignore) continue;
                parser.Feed(token);
            }

            Assert.That(parser.Accepted, Is.True);
        }
        else
        {
            Assert.Catch(() =>
            {
                foreach (var token in _qLexer)
                {
                    if (token.Ignore) continue;
                    parser.Feed(token);
                }
            });
        }
    }
}