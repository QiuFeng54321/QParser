using System.Collections;
using System.Text;
using QParser.Lexer;
using QParser.Lexer.Tokens;
using QParser.Parser;
using QParser.Parser.LR;
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
                new ParserTestCase("3 + + 2.6 * 3.1 4.5 * 6 + (( 3", SupportedParsers.None));
            yield return new TestFixtureData(new TestParser2GrammarConstructor(),
                new ParserTestCase(")32*((3)", SupportedParsers.None));
            yield return new TestFixtureData(new DragonBook4_2_1GrammarConstructor(),
                new ParserTestCase("3 5 * 6 + 7 * 8 9 + 10 * *", SupportedParsers.CanonicalLR | SupportedParsers.SLR));
            yield return new TestFixtureData(new BNF_GrammarConstructor(),
                new ParserTestCase(
                    "<S> = <S> PLUS <E> | <S> MULT <E> | <E>;\n<E> = OPAREN <S> CPAREN | INT;\nINT = 1; PLUS = 2; MULT = 3; OPAREN = 4; CPAREN = 5;",
                    SupportedParsers.CanonicalLR | SupportedParsers.SLR));
        }
    }
}

[Flags]
public enum SupportedParsers
{
    None = 0,
    LL = 1,
    SLR = 2,
    CanonicalLR = 4,
    All = ~(1 << 31)
}

public record ParserTestCase(string Input, SupportedParsers SupportedParsers = SupportedParsers.All);

[TestFixtureSource(typeof(ParserTestsFixtureData), nameof(ParserTestsFixtureData.FixtureParams))]
public class ParserTests
{
    private readonly Grammar _grammar;
    private readonly ParserTestCase _parserTestCase;
    private readonly List<Token> _tokens = new();
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
        var qLexer = new QLexer(stream, _fileInformation);
        _tokens.AddRange(qLexer);
    }

    [Test]
    public void OutputGrammar()
    {
        Console.WriteLine("Original ------------------------------");
        Console.WriteLine(_grammar);
    }

    private void Parse<TParser>(TParser parser, SupportedParsers flag, out bool pass, out bool testResult)
        where TParser : Parser.QParser
    {
        _fileInformation.Exceptions.Clear();
        parser.Generate();
        if (!parser.IsParserValid)
        {
            pass = !_parserTestCase.SupportedParsers.HasFlag(flag);
            testResult = false;
            return;
        }

        parser.DumpTables();

        foreach (var token in _tokens)
        {
            if (token.Ignore) continue;
            TestContext.WriteLine($"Feed {token}");
            parser.Feed(token);
            parser.DumpStates();
        }

        _fileInformation.DumpExceptions();
        TestContext.WriteLine(parser.GetParseTree());
        if (_parserTestCase.SupportedParsers.HasFlag(flag))
            testResult = parser.Accepted && _fileInformation.Exceptions.Count == 0;
        else
            testResult = _fileInformation.Exceptions.Count > 0 || !parser.Accepted;
        pass = false;
    }

    [Test]
    public void ParseLL1()
    {
        var canBeLL1 = _grammar.CanBeLL1();
        if (canBeLL1.IsFailed) Assert.Pass();
        var parser = new LL1Parser(_grammar, _fileInformation);
        Parse(parser, SupportedParsers.LL, out var pass, out var testResult);
        if (pass) Assert.Pass();
        Assert.That(testResult, Is.True);
    }

    [Test]
    public void ParseSLR1()
    {
        var parser = new SLRParser(_grammar, _fileInformation);
        Parse(parser, SupportedParsers.SLR, out var pass, out var testResult);
        if (pass) Assert.Pass();
        Assert.That(testResult, Is.True);
    }

    [Test]
    public void ParseLR1()
    {
        var parser = new CanonicalLRParser(_grammar, _fileInformation);
        Parse(parser, SupportedParsers.CanonicalLR, out var pass, out var testResult);
        if (pass) Assert.Pass();
        Assert.That(testResult, Is.True);
    }
}