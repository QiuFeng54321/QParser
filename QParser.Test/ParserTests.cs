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
                new ParserTestCase("(3 + 5) * 2.6 + 3 * 9.1"));
            yield return new TestFixtureData(new TestParser2GrammarConstructor(),
                new ParserTestCase("3 + + 2.6 * 3.1 4.5 * 6 + (( 3", ParseResult.Failure));
            yield return new TestFixtureData(new TestParser2GrammarConstructor(),
                new ParserTestCase(")32*((3)", ParseResult.Failure));
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
    private readonly Grammar _grammar;
    private readonly ParserTestCase _parserTestCase;
    private readonly QLexer _qLexer;
    private readonly FileInformation _fileInformation;

    public ParserTests(GrammarConstructor grammarConstructor, ParserTestCase parserTestCase)
    {
        grammarConstructor.Construct();
        _grammar = grammarConstructor.Grammar;
        _grammar.GenerateAll();
        _parserTestCase = parserTestCase;
        _fileInformation = new FileInformation("TestFile");
        var stream = new SourceInputStream(_fileInformation,
            new MemoryStream(Encoding.UTF8.GetBytes(parserTestCase.Input)));
        _qLexer = new QLexer(stream, _fileInformation);
    }

    [Test]
    public void OutputGrammar()
    {
        Console.WriteLine("Original ------------------------------");
        Console.WriteLine(_grammar);
    }

    [Test]
    public void ParseLL1()
    {
        var canBeLL1 = _grammar.CanBeLL1();
        if (canBeLL1.IsFailed) Assert.Pass();
        var parser = new LL1Parser(_grammar, _fileInformation);
        parser.DumpTable();

        foreach (var token in _qLexer)
        {
            if (token.Ignore) continue;
            parser.Feed(token);
        }

        _fileInformation.DumpExceptions();
        if (_parserTestCase.Result is ParseResult.Success)
        {
            Assert.That(parser.Accepted, Is.True);
        }
        else
        {
            Assert.That(_fileInformation.Exceptions, Is.Not.Empty);
        }
    }
}